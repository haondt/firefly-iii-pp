from pipe_utils import trampoline_pipe as pipe
from pipe_utils import p_iter_loop as p_iter
from pipe_utils import (
    f_pipe, p_echo, p_tee, p_trace, p_filter, p_if, p_fork, p_map, p_sort, p_id,
    p_append, p_discard, p_first, p_noop, p_reduce, p_unpack,
)

def log(msg, level="info"):
    print(msg)

def p_debug(x):
    log(x)
    return x

def f_debug(f):
    def _inner(*x):
        log(x)
        if len(x) == 1:
            return f(x[0])
        return f(*x)
    return _inner

def t_debug(*x):
    print(x)
    return x
