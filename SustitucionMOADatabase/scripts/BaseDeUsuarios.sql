--Querys general
select * from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)) and UltimoLogin is null

3171

--Solo granos
select * from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 2) and UltimoLogin is null -- not null

1131

--Solo NG
select * from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 3)  and UltimoLogin is null -- not null

1672

--Corredores
select * from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 4 and CodigoProveedor like 'C%') and UltimoLogin is null
365

--Prov Corredor
select * from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 4 and CodigoProveedor not like 'C%')
61




--select Union

select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 2)
UNION
select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 3)
Union
select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 4 and CodigoProveedor like 'C%')


--ver si esta o no

select *
from usuario
where Id not in 
(
select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 2)
UNION
select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 3)
Union
select Id from Usuario where Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37)
and TipoProveedor_Id = 4 and CodigoProveedor like 'C%')
) 
and Mail in (select distinct mail from Proveedor where (FechaSolicitud <= '2021-3-31' or FechaSolicitud is null) 
and (mail not like '%Baufest.com' and mail not like '%molinosagro.com.ar') and Id not in (32, 33, 34, 36, 37))