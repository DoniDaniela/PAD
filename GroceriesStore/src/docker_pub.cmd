docker tag 6f28cbd62d666e5227455581bfc1d99f0d06a80d02c17b3a138312a3251cbb3a eshop/basket.api2:linux-latest

docker tag 554f18ab321edf772080b090911ff457fe85a986c3f7692efb1e07dcd88733e6 eshop/catalog.api2:linux-latest

docker tag eshop/webshoppingagg:linux-latest danieladoni01/groceriestore-webshoppingagg:linux-latest

docker tag eshop/webmvc:linux-latest danieladoni01/groceriestore-webmvc:linux-latest

docker tag eshop/basket.api:linux-latest danieladoni01/groceriestore-basket-api:linux-latest

docker tag eshop/basket.api2:linux-latest danieladoni01/groceriestore-basket-api2:linux-latest

docker tag eshop/catalog.api:linux-latest danieladoni01/groceriestore-catalog-api:linux-latest

docker tag eshop/catalog.api2:linux-latest danieladoni01/groceriestore-catalog-api2:linux-latest

docker tag eshop/identity.api:linux-latest danieladoni01/groceriestore-identity-api:linux-latest

docker push danieladoni01/groceriestore-webshoppingagg:linux-latest

docker push danieladoni01/groceriestore-webmvc:linux-latest

docker push danieladoni01/groceriestore-basket-api:linux-latest

docker push danieladoni01/groceriestore-basket-api2:linux-latest

docker push danieladoni01/groceriestore-catalog-api:linux-latest

docker push danieladoni01/groceriestore-catalog-api2:linux-latest

docker push danieladoni01/groceriestore-identity-api:linux-latest


