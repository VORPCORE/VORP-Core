game 'rdr3'
fx_version 'cerulean'
rdr3_warning 'I acknowledge that this is a prerelease build of RedM, and I am aware my resources *will* become incompatible once RedM ships.'

author 'VORPCORE <https://github.com/VORPCORE>'
description 'VORP Core, the central resource to all of VORP'
version '1.1.320'

-- CLIENT FILES

client_scripts {
  'Vorp.Core.Client.net.dll'
}

files {
  'Newtonsoft.Json.dll',
  'Events.dll',
  'Snowflake.dll',
  'Resources/**/*',
  'ui/**/*'
}

ui_page 'ui/index.html'

-- SERVER FILES

server_scripts {
  'server/*.net.dll',
  'vorpAPI.lua' -- Script to be depricated
}

server_exports {'vorpAPI'}

-- SETTINGS
-- Log Levels; Trace -> Debug -> Info -> Warn -> Error -> All
log_level 'all'
