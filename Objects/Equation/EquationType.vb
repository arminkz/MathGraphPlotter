Public Enum EquationType
    None
    CartesianX     'x=f(y,z)
    CartesianY     'y=f(x,z)
    CartesianZ     'z=f(x,y)
    CartesianW     'w=f(x,y,z)
    CartesianN     'y=f(n) (BETA)
    PolarR         'r=f(t,p)
    PolarT         't=f(r,p)
    PolarP         'p=f(r,t)
    ParametricU    'x=f(u) y=f(u) z=f(u)
    ParametricUV   'x=f(u,v) y=f(u,v) z=f(u,v)
    Parametric4D   'x=f(u,v) y=f(u,v) z=f(u,v) w=f(u,v)
End Enum
