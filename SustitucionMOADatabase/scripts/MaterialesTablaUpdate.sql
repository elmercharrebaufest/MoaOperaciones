update Material set Tabla = 0 where CodigoSap is NULL
update Material set Tabla = 1 where CodigoSap is not NULL