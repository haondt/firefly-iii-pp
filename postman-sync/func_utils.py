import functools, pickle

def memoize(f):
    cache = f.cache = {}
    @functools.wraps(f)
    def wrapper(*args, **kwargs):
        k = pickle.dumps(args, 1) + pickle.dumps(kwargs, 1)
        if k not in cache:
            cache[k] = f(*args, **kwargs)
        return cache[k]
    return wrapper