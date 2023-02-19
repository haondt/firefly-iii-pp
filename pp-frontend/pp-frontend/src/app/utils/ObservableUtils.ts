import { ServiceResponseModel } from "../models/ServiceResponse";

interface CheckArgs<T> {
    success?: (body: T) => void,
    fail?: (error: string) => void,
    finally?: () => void
};

export const checkResult = function<T>(args: CheckArgs<T>): (value: ServiceResponseModel<T>) => void {
    return res => {
        try {
            if (res.success) {
                args.success?.(res.body!);
            } else {
                args.fail?.(res.error!);
            }
        } finally {
            args.finally?.();
        }
    };
}
