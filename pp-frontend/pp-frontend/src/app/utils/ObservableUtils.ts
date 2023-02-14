import { ServiceResponseModel } from "../models/ServiceResponse";

export const checkResult = function<T>(args: {
    success: (body: T) => void,
    fail: (error: string) => void,
    finally: () => void
}): (value: ServiceResponseModel<T>) => void {
    return res => {
        try {
            if (res.success) {
                args.success(res.body!);
            } else {
                args.fail(res.error!);
            }
        } finally {
            args.finally();
        }
    };
}