$ErrorActionPreference = "Stop"

$frontendPath = $PSScriptRoot
$port = 5500

if (-not (Get-Command npx -ErrorAction SilentlyContinue)) {
    throw "Node.js and npx are required. Install Node.js, then run this script again."
}

Start-Process "http://localhost:$port"
npx --yes http-server $frontendPath -p $port -c-1
