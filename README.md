[![Join Discord](https://img.shields.io/discord/704317931453939803?style=flat-square)](https://discord.gg/JgYeAr6Tp6)

## VORP-CORE
This is VORP CORE for RedM made in C# compatible with LUA
[Discord VORP](https://discord.gg/23MPbQ6)

## Requirements
- [ghmattimysql](https://github.com/VORPCORE/ghmattimysql-oxmysql)
- [VORP-Core](https://github.com/VORPCORE/VORP-Core/releases)
- [VORP-Inputs](https://github.com/VORPCORE/VORP-Inputs/releases)
- [VORP-Character](https://github.com/VORPCORE/VORP-Character/releases)

## How to install (Remember to download the lastest releases)
* Download ghmattimysql
* Copy and paste ``ghmattimysql`` folder to ``resources/ghmattimysql``
* Delete file ``resources/ghmattimysql/config.json``
* Add ``set mysql_connection_string "mysql://root:@localhost/vorp?acquireTimeout=60000&connectTimeout=60000"`` to your server.cfg file
* Add ``ensure ghmattimysql`` to your ``server.cfg`` file

* To change the language of the core go to ``resources/vorp_core/Config.json`` and change the default language (Same for other scripts)
* Copy and paste ``vorp_core`` folder to ``resources/[vorp]`` (Same for other scripts)
* Add ensure ``vorp_core`` to your ``server.cfg`` file (Same for other scripts)
* Example Server.cfg

```cfg
set mysql_connection_string "mysql://root:yourDBpassword(If you dont have one, leave this blank)@localhost/vorp?acquireTimeout=60000&connectTimeout=60000"

#These resources will start by default.
stop sessionmanager
stop webadmin
stop monitor
ensure mapmanager
ensure chat
ensure spawnmanager
ensure sessionmanager-rdr3
ensure fivem
#ensure hardcap
ensure rconlog
ensure interiors

#MYSQL
ensure ghmattimysql

#VORP Core
ensure vorp_core
ensure vorp_inputs

#VORP Scripts
ensure vorp_character
ensure vorp_inventory
ensure vorp_metabolism
ensure vorp_clothingstore
ensure vorp_stables
ensure vorp_adminmenu
ensure vorp_stores
ensure vorp_weaponstore
ensure vorp_banks
ensure vorp_barbershops
ensure vorp_cinema
ensure vorp_housing
ensure vorp_postman
ensure vorp_woodcutter
```

## Note

The scripts may have an SQL file, you need to create a database and execute the SQL files.

We recommend to download and add the all the scripts to the server together before creating a character if you are going to test the scripts (Specially for vorp_metabolism and vorp_inventory).
We also recommend using the 2619 version of server artifacts.

Now you are ready!

## Wiki
[Wiki VORP Core](http://docs.vorpcore.com:3000/home)

## Credits
Vespura for his work in c# that has helped me create this great project

[ExtendedMode](https://github.com/extendedmode/extendedmode) We have based our callback system on his callback system

[Redemrp_Respawn](https://github.com/RedEM-RP/redemrp_respawn/blob/d26395a9c19169cdf47ab4d66282f7a1436dc925/client/cl_main.lua#L18) based on his respawn/death system.
