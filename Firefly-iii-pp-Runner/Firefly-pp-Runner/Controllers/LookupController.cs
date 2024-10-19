using Firefly_iii_pp_Runner.Controllers;
using Firefly_pp_Runner.Lookup.Services;
using Firefly_pp_Runner.ModelBinders;
using Firefly_pp_Runner.Models.Lookup.Dtos;
using Firefly_pp_Runner.Models.Lookup.Enums;
using FireflyIIIpp.Core.Exceptions;
using FireflyIIIpp.Core.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Firefly_pp_Runner.Controllers
{
    [Route("api/v2/lookup")]
    public class LookupController(ILookupStoreProvider lookupProvider) : BaseController
    {

        [HttpGet]
        [Route("stores")]
        public IActionResult GetStoreNames()
        {
            return new OkObjectResult(lookupProvider.GetAvailableStores());
        }

        /// <summary>
        /// Workaround needed because json string escaping seems much more robust than url encoding.
        /// See: https://github.com/dotnet/aspnetcore/issues/23633 and https://github.com/dotnet/aspnetcore/issues/11544.
        /// Also because you can't (read: shouldn't) provide a json payload in a GET request.
        /// </summary>
        /// <param name="store"></param>
        /// <param name="lookupAction"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("action/{store}/{lookupAction}")]
        public async Task<IActionResult> TakeAction(string store, [ModelBinder(BinderType = typeof(LookupActionEnumModelBinder<LookupActionEnum>))] LookupActionEnum lookupAction, [FromBody] LookupActionRequestDto dto)
        {
            //var(store = await _kvsFactory.TryGetKeyValueStoreService(store);
            //if (!gotKvs)
            //    return new NotFoundObjectResult(new ExceptionDto
            //    {
            //        StatusCode = (int)HttpStatusCode.NotFound,
            //        Message = "The requested store was not found",
            //        Details = store
            //    });

            //dto.AssertVerifyContentsForAction(lookupAction);
            //switch (lookupAction)
            //{
            //    case LookupActionEnum.GetKeys:
            //        {
            //            var (s, r) = await kvService.GetKeys(dto.Value);
            //            return s ? new OkObjectResult(r) : BuildNotFoundExceptionResultDto(dto.Value);
            //        }
            //    case LookupActionEnum.GetKeyValue:
            //        {
            //            var (s, re, r) = await kvService.GetKeyValue(dto.Key);
            //            return s ? new OkObjectResult(r) : BuildNotFoundExceptionResultDto(re, dto.Key);
            //        }
            //    case LookupActionEnum.GetValueValue:
            //        {
            //            var (s, r) = await kvService.GetValueValue(dto.Value);
            //            return s ? new OkObjectResult(r) : BuildNotFoundExceptionResultDto(dto.Value);
            //        }
            //    case LookupActionEnum.GetKeyValueValue:
            //        {
            //            var (s, re, v, vv) = await kvService.GetKeyValueValue(dto.Key);
            //            return s ? new OkObjectResult(new ValueValueValueDto
            //            {
            //                Value = v,
            //                ValueValue = vv
            //            }) : BuildNotFoundExceptionResultDto(re, dto.Key);
            //        }
            //    case LookupActionEnum.DeleteValue:
            //        await kvService.DeleteValue(dto.Value);
            //        return new OkResult();
            //    case LookupActionEnum.AddKey:
            //        await kvService.AddKey(dto.Key, dto.Value);
            //        return new OkResult();
            //    case LookupActionEnum.DeleteKey:
            //        await kvService.DeleteKey(dto.Key);
            //        return new NoContentResult();
            //    case LookupActionEnum.PutValueValue:
            //        await kvService.UpdateValue(dto.Value, dto.ValueValue);
            //        return new OkResult();
            //    case LookupActionEnum.AutoCompleteValue:
            //        return new OkObjectResult(await kvService.AutocompleteValue(dto.PartialValue));
            //    case LookupActionEnum.None:
            //        throw new ArgumentException(nameof(lookupAction));
            //    default:
            //        throw new Exception($"Unexpeceted action: {lookupAction}.");
            throw new NotImplementedException();
        }

        private IActionResult BuildNotFoundExceptionResultDto(string details)
        {
            return new NotFoundObjectResult(new ExceptionDto
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = "The requested resource was not found",
                Details = details,
                Exception = nameof(NotFoundException)
            });
        }
        private IActionResult BuildNotFoundExceptionResultDto(string message, string details)
        {
            return new NotFoundObjectResult(new ExceptionDto
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = message,
                Details = details,
                Exception = nameof(NotFoundException)
            });
        }
    }
}
