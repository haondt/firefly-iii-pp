using Haondt.Web.Services;

namespace Haondt.Web.Pages
{
    public class DefaultPageEntryFactory(DefaultPageEntryFactoryData factoryData) : IRegisteredPageEntryFactory
    {
        public string Page => factoryData.Page;
        public string ViewPath => factoryData.ViewPath;

        public Task<PageEntry> Create(IPageRegistry pageRegistry, IRequestData data, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
            => Create(pageRegistry, factoryData.ModelFactory(pageRegistry, data), responseOptions);

        public Task<PageEntry> Create(IPageRegistry pageRegistry, IPageModel model, Func<HxHeaderBuilder, HxHeaderBuilder>? responseOptions = null)
        {
            return Task.FromResult(new PageEntry
            {
                Page = Page,
                ViewPath = ViewPath,
                Model = model,
                ConfigureResponse = CombineResponseOptions(factoryData.ConfigureResponse, responseOptions)
            });
        }

        /// <summary>
        /// Will be applied in order of arguments (starting with first)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        private static Func<T, T>? CombineNullableFunctions<T>(params Func<T, T>?[] funcs)
        {
            Func<T, T>? combinedFunc = null;
            foreach (var func in funcs)
            {
                if (func == null)
                    continue;
                if (combinedFunc == null)
                    combinedFunc = func;
                else
                    combinedFunc = t => func(t);
            }

            return combinedFunc;
        }

        /// <summary>
        /// Will be applied in order of arguments (starting with first)
        /// </summary>
        private static Action<IHeaderDictionary>? CombineResponseOptions(params Func<HxHeaderBuilder, HxHeaderBuilder>?[] funcs)
        {
            var finalFunc = CombineNullableFunctions(funcs);
            return finalFunc?.Invoke(new HxHeaderBuilder()).Build();
        }


    }
}
