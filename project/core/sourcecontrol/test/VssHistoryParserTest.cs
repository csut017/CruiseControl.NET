using System;
using System.IO;
using NUnit.Framework;

namespace tw.ccnet.core.sourcecontrol.test
{
	[TestFixture]
	public class VssHistoryParserTest
	{
		VssHistoryParser _parser;

		[SetUp]
		public void SetUp()
		{
			_parser = new VssHistoryParser();
		}

		public void AssertEquals(Modification expected, Modification actual)
		{
			Assertion.AssertEquals(expected.Comment, actual.Comment);
			Assertion.AssertEquals(expected.EmailAddress, actual.EmailAddress);
			Assertion.AssertEquals(expected.FileName, actual.FileName);
			Assertion.AssertEquals(expected.FolderName, actual.FolderName);
			Assertion.AssertEquals(expected.ModifiedTime, actual.ModifiedTime);
			Assertion.AssertEquals(expected.Type, actual.Type);
			Assertion.AssertEquals(expected.UserName, actual.UserName);
			Assertion.AssertEquals(expected, actual);
		}

		public void TestParse()
		{
			Modification[] mods = _parser.Parse(VssMother.ContentReader);
			Assertion.AssertNotNull("mods should not be null", mods);
			Assertion.AssertEquals(19, mods.Length);			
		}

		[Test]
		public void ReadAllEntriesTest() 
		{
			string[] entries = _parser.ReadAllEntries(VssMother.ContentReader);
			Assertion.AssertEquals(24, entries.Length);
		}

		public void TestIsEntryDelimiter()
		{
			string line = "*****  cereal.txt  *****";
			Assertion.Assert("should recognize as delim", _parser.IsEntryDelimiter(line));

			line = "*****************  Version 8   *****************";
			Assertion.Assert("should recognize as delim", _parser.IsEntryDelimiter(line));

			line = "*****";
			Assertion.Assert(String.Format("should not recognize as delim '{0}'", line), _parser.IsEntryDelimiter(line) == false);

			line = "*****************  Version 4   *****************";
			Assertion.Assert("should recognize as delim", _parser.IsEntryDelimiter(line));
		}

		public void TestParseCreatedModification()
		{
			string entry = EntryWithSingleLineComment();
			
			Modification expected = new Modification();
			expected.Comment = "added subfolder";
			expected.UserName = "Admin";
			expected.ModifiedTime = new DateTime(2002, 9, 16, 14, 41, 0);
			expected.Type = "Created";
			expected.FileName = "[none]";
			expected.FolderName = "plant";

			Modification[] actual = _parser.parseModifications(makeArray(entry));
			Assertion.AssertNotNull("expected a mod", actual);
			Assertion.AssertEquals("created should not have produced a modification", 0, actual.Length);
		}

		public void TestParseUsernameAndDate()
		{
			Modification mod = new Modification();
			
			string line = "foo\r\nUser: Admin        Date:  9/16/02   Time:  2:40p\r\n";
			CheckInParser parser = new CheckInParser(line);
			parser.ParseUsernameAndDate(mod);
			string expectedUsername = "Admin";
			DateTime expectedDate = new DateTime(2002, 09, 16, 14, 40, 0);
			Assertion.AssertEquals(expectedUsername, mod.UserName);
			Assertion.AssertEquals(expectedDate, mod.ModifiedTime);
		}

		[Test]
		public void TestParseUsernameAndDateWithPeriod() 
		{
			//User: Gabriel.gilabert     Date:  5/08/03   Time:  4:06a
			Modification mod = new Modification();
			
			string line = "foo\r\nUser: Gabriel.gilabert     Date:  5/08/03   Time:  4:06a\r\n";
			CheckInParser parser = new CheckInParser(line);
			parser.ParseUsernameAndDate(mod);
			string expectedUsername = "Gabriel.gilabert";
			DateTime expectedDate = new DateTime(2003, 05, 08, 04, 06, 0);
			Assertion.AssertEquals(expectedUsername, mod.UserName);
			Assertion.AssertEquals(expectedDate, mod.ModifiedTime);
		}

		[Test]
		public void TestParseFileName() 
		{
			string fileName = "**** Im a file name.fi     ********\r\n jakfakjfnb  **** ** lkjnbfgakj ****";
			CheckInParser parser = new CheckInParser(fileName);
			string actual = parser.parseFileName();
			Assertion.AssertEquals("Im a file name.fi", actual);
		}

		public void TestParseFileAndFolder_checkin()
		{
			string entry = @"*****  happyTheFile.txt  *****
Version 3
User: Admin        Date:  9/16/02   Time:  5:01p
Checked in $/you/want/folders/i/got/em
Comment: added fir to tree file, checked in recursively from project root";

			string expectedFile = "happyTheFile.txt";
			string expectedFolder = "$/you/want/folders/i/got/em";

			Modification mod = ParseAndAssertFilenameAndFolder(entry, expectedFile, expectedFolder);
			Assertion.AssertEquals("Admin", mod.UserName);
			Assertion.AssertEquals(new DateTime(2002, 9, 16, 17, 01, 0), mod.ModifiedTime);
			Assertion.AssertEquals("checkin", mod.Type);
			Assertion.AssertEquals("added fir to tree file, checked in recursively from project root",mod.Comment);
		}

		public void TestParseFileAndFolder_addAtRoot()
		{
			// note: this represents the entry after version line insertion 
			// (see _parser.InsertVersionLine)
			string entry = @"*****************  Version 2   *****************
Version 2
User: Admin        Date:  9/16/02   Time:  2:40p
happyTheFile.txt added
";
			string expectedFile = "happyTheFile.txt";
			string expectedFolder = "[projectRoot]";

			Modification mod = ParseAndAssertFilenameAndFolder(entry, expectedFile, expectedFolder);
			Assertion.AssertEquals("Admin", mod.UserName);
			Assertion.AssertEquals(new DateTime(2002, 9, 16, 14, 40, 0), mod.ModifiedTime);
			Assertion.AssertEquals("added", mod.Type);
			Assertion.AssertEquals(null, mod.Comment);
		}
		
		public void TestParseFileAndFolder_deleteFromSubfolder()
		{
string entry = @"*****  iAmAFolder  *****
Version 8
User: Admin        Date:  9/16/02   Time:  5:29p
happyTheFile.txt deleted";

			string expectedFile = "happyTheFile.txt";
			string expectedFolder = "iAmAFolder";

			Modification mod = ParseAndAssertFilenameAndFolder(entry, expectedFile, expectedFolder);
			Assertion.AssertEquals("Admin", mod.UserName);
			Assertion.AssertEquals(new DateTime(2002, 9, 16, 17, 29, 0), mod.ModifiedTime);
			Assertion.AssertEquals("deleted", mod.Type);
			Assertion.AssertEquals(null, mod.Comment);
		}

		private string[] makeArray(params string[] entries) 
		{
			return entries;
		}

		private Modification ParseAndAssertFilenameAndFolder(
			string entry, string expectedFile, string expectedFolder)
		{
			string[] entries = makeArray(entry);

			Modification[] mod = _parser.parseModifications(entries);
			
			Assertion.AssertNotNull(mod);
			Assertion.AssertEquals(1, mod.Length);
			Assertion.AssertEquals(expectedFile, mod[0].FileName);
			Assertion.AssertEquals(expectedFolder, mod[0].FolderName);

			return mod[0];
		}

		public void TestParseSingleLineComment()
		{
			CheckInParser parser = new CheckInParser(EntryWithSingleLineComment());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals("a", "added subfolder", mod.Comment);
		}

		public void TestParseMultiLineComment()
		{
			CheckInParser parser = new CheckInParser(EntryWithMultiLineComment());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals("b", @"added subfolder
and then added a new line", mod.Comment);
		}

		public void TestParseEmptyComment()
		{
			CheckInParser parser = new CheckInParser(EntryWithEmptyComment());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals(String.Empty, mod.Comment);
		}

		public void TestParseEmptyLineComment()
		{
			CheckInParser parser = new CheckInParser(EntryWithEmptyCommentLine());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals(null, mod.Comment);
		}

		public void TestParseNoComment()
		{
			CheckInParser parser = new CheckInParser(EntryWithNoCommentLine());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals(null, mod.Comment);
		}

		public void TestParseNonCommentAtCommentLine()
		{
			CheckInParser parser = new CheckInParser(EntryWithNonCommentAtCommentLine());
			Modification mod = new Modification();
			parser.ParseComment(mod);
			Assertion.AssertEquals(null, mod.Comment);
		}

		private TextReader TwoEntries()
		{
			string entries = 
				@"*****  plant  *****
Version 1
User: Admin        Date:  9/16/02   Time:  2:41p
Created
Comment: added subfolder

*****************  Version 4   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
toast.txt added";
			return new StringReader(entries);					
		}
		
		private string EntryWithSingleLineComment()
		{
			string entry = 
				@"*****  plant  *****
Version 1
User: Admin        Date:  9/16/02   Time:  2:41p
Created
Comment: added subfolder";
			return entry;
		}

		private string EntryWithMultiLineComment()
		{
			return @"*****  plant  *****
Version 1
User: Admin        Date:  9/16/02   Time:  2:41p
Created
Comment: added subfolder
and then added a new line";
		}

		private string EntryWithEmptyComment()
		{
return @"*****************  Version 1   *****************
User: Admin        Date:  9/16/02   Time:  2:29p
Created
Comment: 

";
		}

		private string EntryWithEmptyCommentLine()
		{
return @"*****************  Version 2   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
jam.txt added

";
		}

		private string EntryWithNoCommentLine()
		{
return @"*****************  Version 2   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
jam.txt added";
		}
			
		private string EntryWithNonCommentAtCommentLine()
		{
return @"*****************  Version 2   *****************
User: Admin        Date:  9/16/02   Time:  2:40p
jam.txt added
booya, grandma, booya";
		}

		private string[] EntryLines(string input)
		{
			return input.Split('\n');
		}
	}
}
