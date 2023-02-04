export const dict = function<T>(arr: {key: string, value: T}[]): {[key: string]: T} {
    return arr.reduce<{[key: string]: T}>((d, x) => (d[x.key] = x.value, d), {});
}