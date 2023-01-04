import functools
def p_reduce(f, seed):
    return lambda x: functools.reduce(f, x, seed)

def p_map(f):
    return lambda x: map(f, x)

# unpack the result of the previous function as arguments for the next
def p_unpack(f):
    return lambda x: f(*x)

def p_fork(f1, f2):
    def _inner(*args, **kwargs):
        return (
            f1(*args, **kwargs),
            f2(*args, **kwargs)
        )
    return _inner

def p_append(y):
    return lambda x: x + (y,)

# probably not a good sign
def p_tee(f):
    def _inner(*args, **kwargs):
        f(*args, **kwargs)
        return args[0]
    return _inner

def p_if(f_condition, f_true, f_false):
    def _inner(*args, **kwargs):
        if f_condition(*args, **kwargs):
            return f_true(*args, **kwargs)
        return f_false(*args, **kwargs)
    return _inner