using Firefly_pp_Runner.Models.Lookup.Dtos;
using Firefly_pp_Runner.Models.Lookup.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Extensions
{
    public static class LookupActionRequestDtoExtensions
    {
        public static void AssertVerifyContentsForAction(this LookupActionRequestDto dto, LookupActionEnum lookupAction)
        {
            switch (lookupAction)
            {
                case LookupActionEnum.GetKeys:
                case LookupActionEnum.GetValueValue:
                case LookupActionEnum.DeleteValue:
                    if (string.IsNullOrEmpty(dto.Value))
                        throw new ArgumentException(nameof(dto.Value));
                    break;
                case LookupActionEnum.AddKey:
                    if (string.IsNullOrEmpty(dto.Value))
                        throw new ArgumentException(nameof(dto.Value));
                    if (string.IsNullOrEmpty(dto.Key))
                        throw new ArgumentException(nameof(dto.Key));
                    break;
                case LookupActionEnum.GetKeyValueValue:
                case LookupActionEnum.GetKeyValue:
                case LookupActionEnum.DeleteKey:
                    if (string.IsNullOrEmpty(dto.Key))
                        throw new ArgumentException(nameof(dto.Key));
                    break;
                case LookupActionEnum.PutValueValue:
                    if (string.IsNullOrEmpty(dto.Value))
                        throw new ArgumentException(nameof(dto.Value));
                    if (string.IsNullOrEmpty(dto.ValueValue))
                        throw new ArgumentException(nameof(dto.ValueValue));
                    break;
                case LookupActionEnum.AutoCompleteValue:
                case LookupActionEnum.None:
                    break;
                default:
                    throw new Exception($"Unexpeceted action: {lookupAction}.");
            }
        }
    }
}
