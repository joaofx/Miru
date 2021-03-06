using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Miru.Html;
using NUnit.Framework;
using ModelMetadataProviderExtensions = Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadataProviderExtensions;

namespace Miru.Tests.Html.TagHelpers
{
    public class TagHelperTest
    {
        protected IServiceProvider ServiceProvider { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var services = new ServiceCollection()
                .AddMiruHtml(new HtmlConvention().AddTwitterBootstrap())
                .AddOptions()
                .AddLogging();
            
            services.AddMvcCore().AddViews();
                
            ServiceProvider = services.BuildServiceProvider();
        }

        protected TagHelperOutput ProcessTagSync<TTag>(TTag tag, string html, string childContent = "") where TTag : TagHelper
        {
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var output = new TagHelperOutput(html,
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(childContent);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            tag.Process(context, output);

            return output;
        }
        
        protected async Task<TagHelperOutput> ProcessTag<TTag>(TTag tag, string tagName, string childContent = "") where TTag : TagHelper
        {
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
            
            var output = new TagHelperOutput(tagName,
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(childContent);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            await tag.ProcessAsync(context, output);

            return output;
        }

        protected ModelExpression MakeExpression<TModel>(TModel viewModel, string propertyName, object propertyContent)
        {
            var containerType = viewModel.GetType();

            var m3 = ServiceProvider.GetService<ModelExpressionProvider>();
            
            var compositeMetadataDetailsProvider = ServiceProvider.GetService<ICompositeMetadataDetailsProvider>();
            var metadataProvider = new DefaultModelMetadataProvider(compositeMetadataDetailsProvider);

            var containerMetadata = metadataProvider.GetMetadataForType(containerType);
            var containerExplorer = metadataProvider.GetModelExplorerForType(containerType, viewModel);

            var propertyMetadata = metadataProvider.GetMetadataForProperty(containerType, propertyName);
            
            var modelExplorer = containerExplorer.GetExplorerForExpression(propertyMetadata, propertyContent);

            return new ModelExpression(propertyName, modelExplorer);
        }
        
        protected ModelExpression MakeExpression<TModel, TProperty>(
            TModel model, 
            Expression<Func<TModel, TProperty>> expression)
        {
            var modelExpressionProvider = ServiceProvider.GetService<ModelExpressionProvider>();
                
            var compositeMetadataDetailsProvider = ServiceProvider.GetService<ICompositeMetadataDetailsProvider>();
            var metadataProvider = new DefaultModelMetadataProvider(compositeMetadataDetailsProvider);

            var viewDataDictionary = new ViewDataDictionary<TModel>(metadataProvider, new ModelStateDictionary())
            {
                Model = model
            };

            return modelExpressionProvider.CreateModelExpression(viewDataDictionary, expression);
        }
    }
}