import functools

# some pipe-friendly tools

def p_reduce(f, seed):
    return lambda x: functools.reduce(f, x, seed)

def p_map(f):
    return lambda x: map(f, x)

def p_filter(f):
    return lambda x: filter(f, x)

def p_id(x):
    return x

def p_sort(f):
    return lambda x: sorted(x, key=f)

def p_noop(*_):
    return None

# inject args objects into pipeline
def p_echo(*x):
    return lambda *_: (x[0] if len(x) == 1 else (*x,))

# first item matching the condition
def p_first(f):
    return lambda x: next(filter(f, x))


# recursive iteration
def p_iter(f):
    def _inner(it):
        if len(it) > 0:
            f(it[0])
            return _inner(it[1:])
        return None
    return _inner

# unpack the arguments before sending them to the next function
def p_unpack(f):
    return lambda x: f(*x)

# run function and return result from previous function
def p_tee(f):
    def _inner(*args, **kwargs):
        f(*args, **kwargs)
        return args[0]
    return _inner

# discard output of previous function
def p_discard(f):
    return lambda *_: f()

# returns input as well as output of supplied function
def p_trace(f):
    return lambda *x: (x[0] if len(x) == 1 else (*x,), f(*x))


# return the results of two functions
def p_fork(f1, f2):
    def _inner(*args, **kwargs):
        return (
            f1(*args, **kwargs),
            f2(*args, **kwargs)
        )
    return _inner

def p_append(y):
    return lambda x: x + [y]

def p_if(f_condition, f_true, f_false=p_noop):
    def _inner(*args, **kwargs):
        if f_condition(*args, **kwargs):
            return f_true(*args, **kwargs)
        return f_false(*args, **kwargs)
    return _inner


# pipelines

def pipe(*funcs):
    def _inner(*args, **kwargs):
        def __inner(fs, *args, **kwargs):
            if len(fs) == 0:
                return args
            return __inner(fs[1:], fs[0](*args, **kwargs))
        return __inner(funcs, *args, **kwargs)[0]
    return _inner

# continuation passing pipe
def cont_pipe(*funcs):
    if len(funcs) == 1:
        return funcs[0]
    return lambda *x: cont_pipe(*funcs[1:])(funcs[0](*x))

# function pipe / backwards pipe
# e.g. foo(bar(baz)) = f_pipe(foo, bar, baz)
# as opposed to regular pipe
# foo(bar(baz)) = pipe(bar, foo)(baz)
# or
# foo(bar(baz)) = pipe(p_echo(baz), bar, foo)
def f_pipe(*funcs):
    if len(funcs) == 1:
        return funcs[0]
    return funcs[0](f_pipe(*funcs[1:]))


# tfw no TCO
# see: http://neopythonic.blogspot.com/2009/04/final-words-on-tail-calls.html

# trampoline pipe
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

# iterative loop
def p_iter_loop(f):
    def _inner(it):
        for i in it:
            f(i)
        return None
    return _inner