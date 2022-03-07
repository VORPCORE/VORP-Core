game 'rdr3'
fx_version 'cerulean'
rdr3_warning 'I acknowledge that this is a prerelease build of RedM, and I am aware my resources *will* become incompatible once RedM ships.'

author 'VORPCORE <https://github.com/VORPCORE>'
description 'VORP Core, the central resource to all of VORP'
version '1.1.320'

client_scripts {
  'Vorp.Core.Client.net.dll'
}

server_scripts {
  'server/*.net.dll',
  'vorpAPI.lua' -- Script to be depricated
}

server_exports {'vorpAPI'}


files {
  'Newtonsoft.Json.dll',
  'Events.dll',
  'Snowflake.dll',
  'Resources/**/*',
  'ui/**/*'
}