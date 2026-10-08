import { dotnet } from './_framework/dotnet.js'

const { setModuleImports, getAssemblyExports, getConfig, runMainAndExit } = await dotnet
    .withDiagnosticTracing(false)
    .create();

setModuleImports('main.mjs', {
    node: {
        process: {
            version: () => globalThis.process.version
        }
    }
});

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
const text = exports.Lynx.Wasm.UciInterop.Initialize();
console.log(text);

// run the C# Main() method and exit the process
await runMainAndExit();
