import pickle
import functools

# some pipe-friendly tools

def p_reduce(f, seed):
    return lambda x: functools.reduce(f, x, seed)

def p_map(f):
    return lambda x: map(f, x)

# echo args
def p_echo(*x):
    return lambda: (*x,)

# run function and return result from previous function
def p_tee(f):
    def _inner(*args, **kwargs):
        f(*args, **kwargs)
        return args[0]
    return _inner

# unpack the arguments before sending them to the function
def p_unpack(f):
    return lambda x: f(*x)

# return the results of two functions
def p_fork(f1, f2):
    def _inner(*args, **kwargs):
        return (
            f1(*args, **kwargs),
            f2(*args, **kwargs)
        )
    return _inner

def p_append(y):
    return lambda x: x + (y,)

def p_if(f_condition, f_true, f_false):
    def _inner(*args, **kwargs):
        if f_condition(*args, **kwargs):
            return f_true(*args, **kwargs)
        return f_false(*args, **kwargs)
    return _inner

# decorators

def memoize(f):
    cache = f.cache = {}
    @functools.wraps(f)
    def wrapper(*args, **kwargs):
        k = pickle.dumps(args, 1) + pickle.dumps(kwargs, 1)
        if k not in cache:
            cache[k] = f(*args, **kwargs)
        return cache[k]
    return wrapper

# pipelines

def pipe(*funcs):
    def _inner(*args, **kwargs):
        def __inner(fs, *args, **kwargs):
            if len(fs) == 0:
                return args
            return __inner(fs[1:], fs[0](*args, **kwargs))
        return __inner(funcs, *args, **kwargs)[0]
    return _inner

# trampoline pipe (constant stack frames)
def nest(f, next_f):
    def _inner(*args):
        return next_f, (f(*args),)
    return _inner
def build_pipe(*funcs):
    if len(funcs) > 0:
        return nest(funcs[0], build_pipe(*funcs[1:]))
    return lambda x: (None, x)
def run_pipe(p):
    f, a = p
    while True:
        if f == None:
            break
        f, a = f(*a)
    return a
def trampoline_pipe(*funcs):
    return lambda *x: run_pipe((build_pipe(*funcs), x))

# continuation passing pipe
def cont_pipe(*funcs):
    if len(funcs) == 1:
        return funcs[0]
    return lambda *x: cont_pipe(*funcs[1:])(funcs[0](*x))



