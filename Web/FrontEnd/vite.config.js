import vue from '@vitejs/plugin-vue'
import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'

import { defineConfig } from 'vite'
import { fileURLToPath, URL } from 'node:url'

import appsettings from "../appsettings.json";
import appsettingsDev from "../appsettings.Development.json";

const path = require('path');

const { spawn } = require('child_process');

const fs = require('fs');


const baseFolder =
process.env.APPDATA !== undefined && process.env.APPDATA !== ''
? `${process.env.APPDATA}/ASP.NET/https`
: `${process.env.HOME}/.aspnet/https`;

const certificateArg = process.argv.map(arg => arg.match(/--name=(?<value>.+)/i)).filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "FrontEnd";


const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!certificateName) {
  console.error('Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.');
  process.exit(-1);
}


export default defineConfig(async () => {
  
  if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    await new Promise((resolve) => {
      spawn('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
      ], { stdio: 'inherit', })
          .on('exit', (code) => {
            resolve();
            if (code) {
              process.exit(code);
            }
          });
    });
  };
  

  // Define Vite configuration
  const config = {
    plugins: [
      vue({
        template: { transformAssetUrls }
      }),
      vuetify({
        autoImport: true,
        styles: {
          configFile: 'src/styles/settings.scss',
        },
      })
    ],
    define: { 'process.env': {} },
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      },
      extensions: [
        '.js',
        '.json',
        '.jsx',
        '.mjs',
        '.ts',
        '.tsx',
        '.vue',
      ],
    },
    
    //clearScreen: true,
    //appType: 'mpa',
    //root: "FrontEnd2",
    
    publicDir: 'public',
    build: {
      manifest: appsettings?.Vite?.Manifest,
      outDir: path.resolve(__dirname, "../wwwroot"),
      emptyOutDir: true,
      /*
      assetsDir: 'src/assets',
      rollupOptions: {
        input: ['src/main.js', "src/styles/settings.scss" ],
        output: {
          entryFileNames: 'js/[name].js',
          chunkFileNames: 'js/[name]-chunk.js',
          assetFileNames: (info) => {
            if (info.name) {
              if (cssPattern.test(info.name)) {
                return 'css/[name][extname]';
              }
              if (imagePattern.test(info.name)) {
                return 'images/[name][extname]';
              }
              
              return 'assets/[name][extname]';
            } else {
              return '[name][extname]';
            }
          },
        }
      },
      */
    },
    server: {
      port: appsettingsDev.Vite.Server.Port,
      strictPort: true,
      cors:{
        "origin": "*",
        //"methods": "GET,HEAD,PUT,PATCH,POST,DELETE",
        "preflightContinue": true,
        //"optionsSuccessStatus": [204, 200]
      },
      https: {
        cert: certFilePath,
        key: keyFilePath
      },
      
      hmr: true,//AutoRefresh
      /*
      { 
        host: "localhost",
        clientPort: appsettingsDev.Vite.Server.Port
      },
      */
      //open: true, //Abrir navegador ao iniciar
      allowedHosts: "*",
      //origin: "*"
    }    
  }

  return config;
}

/*
  
  {
  plugins: [
    vue({
      template: { transformAssetUrls }
    }),
    // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
    vuetify({
      autoImport: true,
      styles: {
        configFile: 'src/styles/settings.scss',
      },
    })
  ],
  define: { 'process.env': {} },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
    extensions: [
      '.js',
      '.json',
      '.jsx',
      '.mjs',
      '.ts',
      '.tsx',
      '.vue',
    ],
  },
  build: {
    outDir: path.resolve(__dirname, "../BackEnd/wwwroot")
  },
  server: {
    https: true,
    port: 5002,
  },
  https: {
    key: fs.readFileSync(keyFilePath),
    cert: fs.readFileSync(certFilePath)
  }
  
}
*/
)
