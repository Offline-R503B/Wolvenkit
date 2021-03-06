@echo off

@RD /S /Q .\publish\app\full
@RD /S /Q .\publish\console\full

echo "publish app"
dotnet publish .\WolvenKit\WolvenKit.UI.csproj -o publish\app\full --no-self-contained -r win-x64 -c Release -f net5.0-windows

echo "publish console"
dotnet publish .\WolvenKit.CLI\WolvenKit.CLI.csproj -o publish\console\full --no-self-contained -r win-x64 -c Release -f net5.0

echo "The program has completed"