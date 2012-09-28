﻿namespace CruiseControlNet.SelfHost.Controllers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Xsl;
    using CruiseControlNet.SelfHost.Helpers;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using BuildSummary = CruiseControlNet.SelfHost.Models.BuildSummary;

    /// <summary>
    /// Exposes build information.
    /// </summary>
    public class BuildController
        : ApiControllerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController" /> class.
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        public BuildController(ICruiseServer cruiseServer)
            : base(cruiseServer)
        {
        }
        #endregion

        #region Public methods
        #region Index()
        /// <summary>
        /// Gets the builds for a project.
        /// </summary>
        /// <param name="project">The project identifier (name).</param>
        /// <returns>
        /// The build details.
        /// </returns>
        [HttpGet]
        public IQueryable<BuildSummary> Index(string project)
        {
            this.ValidateServer();

            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var builds = this.CruiseServer.GetBuildNames(new ProjectRequest(null, entity.Name));
            return builds.Data
                .Select(b => new LogFile(b))
                .Select(b => b.ToModel()).AsQueryable();
        }
        #endregion

        #region Details()
        /// <summary>
        /// Gets the details on a build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The build details.
        /// </returns>
        [HttpGet]
        public BuildSummary Details(string project, string id)
        {
            this.ValidateServer();

            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = new LogFile(id).ToModel();
            var result = this.CruiseServer.GetFinalBuildStatus(new BuildRequest(null, entity.Name) { BuildName = id });
            model.Tasks = result.Snapshot.ToModel();
            return model;
        }
        #endregion

        #region Log()
        /// <summary>
        /// Gets the log for a build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The log.
        /// </returns>
        [HttpGet]
        public BuildSummary Log(string project, string id)
        {
            return this.Log(project, id, null);
        }   

        /// <summary>
        /// Gets the log for a build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="id">The id.</param>
        /// <param name="format">The format.</param>
        /// <returns>
        /// The log.
        /// </returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [HttpGet]
        public BuildSummary Log(string project, string id, string format)
        {
            this.ValidateServer();

            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = new LogFile(id).ToModel();
            var result = this.CruiseServer.GetLog(new BuildRequest(null, entity.Name) { BuildName = id });
            if (string.IsNullOrEmpty(format))
            {
                model.Log = result.Data;
            }
            else
            {
                var xslt = new XslCompiledTransform();
                var path = Path.Combine(Environment.CurrentDirectory, "Templates", format + ".xsl");
                if (!File.Exists(path))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        xslt.Load(reader);
                    }
                }

                var xml = new XmlDocument();
                xml.LoadXml(result.Data);
                var builder = new StringBuilder();
                using (var output = XmlWriter.Create(builder))
                {
                    xslt.Transform(xml, output);
                }

                model.Log = builder.ToString();
            }

            return model;
        }
        #endregion
        #endregion
    }
}