using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.TeachInFurtherEducation.Web.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SFA.DAS.TeachInFurtherEducation.Web.Services
{

    public class ViewRenderService : IViewRenderService
    {

        #region Properties

        private readonly ILogger<ViewRenderService> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly ICompositeViewEngine _viewEngine;

        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructors

        public ViewRenderService(
            
            ILogger<ViewRenderService> logger,

            IServiceProvider serviceProvider,

            ICompositeViewEngine viewEngine,

            IHttpContextAccessor httpContextAccessor

        ) 
        {

            _logger = logger;

            _serviceProvider = serviceProvider;

            _viewEngine = viewEngine;

            _httpContextAccessor = httpContextAccessor;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Render partial views as string for output to the front end.
        /// </summary>
        /// <typeparam name="TModel">Output type to render the view as.</typeparam>
        /// <param name="viewName">Specifide name of the partial view.</param>
        /// <param name="model">Output type to render the view as.</param>
        /// <returns>System.string rendered html output of the partial view.</returns>
        public async Task<string> RenderToStringAsync<TModel>(string viewName, TModel model)
        {

            try
            {

                var httpContext = _httpContextAccessor.HttpContext;

                #pragma warning disable CS8604 // Possible null reference argument.

                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

                #pragma warning restore CS8604 // Possible null reference argument.

                using (var sw = new StringWriter())
                {

                    var viewResult = _viewEngine.FindView(actionContext, viewName, false);

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"{viewName} does not match any available view");
                    }

                    var viewData = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {

                        Model = model

                    };

                    var viewContext = new ViewContext(

                        actionContext,

                        viewResult.View,

                        viewData,

                        new TempDataDictionary(actionContext.HttpContext, _serviceProvider.GetRequiredService<ITempDataProvider>()),

                        sw,

                        new HtmlHelperOptions()

                    );

                    await viewResult.View.RenderAsync(viewContext);

                    return sw.ToString();

                }

            }
            catch(Exception _exception)
            {

                _logger.LogError(_exception, "Unable to render partial view {viewName}", viewName);

                return string.Empty;

            }

        }

        #endregion

    }

}
