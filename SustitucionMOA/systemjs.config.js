/**
 * System configuration for Angular samples
 * Adjust as necessary for your application needs.
 */
(function (global) {
    System.config({
        paths: {
            // paths serve as alias
            "npm:": "/node_modules/",
        },
        meta: {
            xlsx: {
                exports: "XLSX", // <-- tell SystemJS to expose the XLSX variable
            },
        },
        // map tells the System loader where to look for things
        map: {
            // our app is within the app folder
            app: "src/app",

            // angular bundles
            "@angular/core": "npm:@angular/core/bundles/core.umd.js",
            "@angular/common": "npm:@angular/common/bundles/common.umd.js",
            "@angular/compiler":
                "npm:@angular/compiler/bundles/compiler.umd.js",
            "@angular/platform-browser":
                "npm:@angular/platform-browser/bundles/platform-browser.umd.js",
            "@angular/platform-browser-dynamic":
                "npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.min.js",
            "@angular/http": "npm:@angular/http/bundles/http.umd.js",
            "@angular/router": "npm:@angular/router/bundles/router.umd.js",
            "@angular/router/upgrade":
                "npm:@angular/router/bundles/router-upgrade.umd.js",
            "@angular/forms": "npm:@angular/forms/bundles/forms.umd.js",
            "@angular/upgrade": "npm:@angular/upgrade/bundles/upgrade.umd.js",
            "@angular/upgrade/static":
                "npm:@angular/upgrade/bundles/upgrade-static.umd.js",

            // other libraries
            //'rx': 'node_modules/rx/dist/rx.js',
            // 'rx': 'npm:rxjs',
            // 'rxjs': './node_modules/rxjs',

            // 'rxjs-compat': 'npm:rxjs-compat',
            // 'rxjs/internal-compatibility' : 'npm:rxjs/internal-compatibility/index.js',

            rxjs: "npm:rxjs",
            "rxjs/operators": "npm:rxjs/operators",
            "rxjs-compat/add/observable": "npm:rxjs-compat/add/observable",
            "rxjs-compat/add/operator": "npm:rxjs-compat/add/operator",
            "rxjs-compat/operator": "npm:rxjs-compat/operator",
            //'rxjs/add/observable/*' : ['npm:rxjs/add/observable/*'],
            //'./Rx': 'node_modules/rx/dist/rx.js',
            "angular-ng-autocomplete":
                "npm:angular-ng-autocomplete/bundles/angular-ng-autocomplete.umd.js",
            "ngx-pagination": "npm:ngx-pagination/dist/ngx-pagination.umd.js",
            "ng2-select": "npm:ng2-select",
            "ng2-auto-complete": "node_modules/ng2-auto-complete/dist",
            "angular2-recaptcha": "node_modules/angular2-recaptcha",
        },
        // packages tells the System loader how to load when no filename and/or no extension
        packages: {
            app: {
                main: "./src/main.js",
                defaultExtension: "js",
            },
            // Rx: {
            //     main: 'Rx.js',
            //     defaultExtension: 'js'
            // },
            // rxjs: {
            //     main: 'rx.js',
            //     defaultExtension: 'js'
            // },
            // 'rxjs-compat':{
            //     main: 'umd.js',
            //     defaultExtension: 'js'
            // },
            rxjs: {
                defaultExtension: "js",
                main: "index.js",
            },
            "rxjs-compat": { defaultExtension: "js", main: "index.js" },
            "rxjs/operators": { main: "index.js", defaultExtension: "js" },
            "rxjs/observable": { defaultExtension: "js" },
            "rxjs-compat/add/observable": { defaultExtension: "js" },
            "rxjs-compat/add/operator": { defaultExtension: "js" },
            "rxjs-compat/operator": { defaultExtension: "js" },
            "rxjs/internal-compatibility": {
                main: "index.js",
                defaultExtension: "js",
            },
            "rxjs/testing": { main: "index.js", defaultExtension: "js" },
            "rxjs/ajax": { main: "index.js", defaultExtension: "js" },
            "rxjs/webSocket": { main: "index.js", defaultExtension: "js" },
            "ngx-pagination": {
                defaultExtension: "js",
            },
            "ng2-select": {
                main: "ng2-select.js",
                defaultExtension: "js",
            },
            "ngx-modal": {
                main: "index.js",
                defaultExtension: "js",
            },
            "ng2-auto-complete": {
                main: "ng2-auto-complete.umd.js",
                defaultExtension: "js",
            },
            "angular2-recaptcha": {
                defaultExtension: "js",
                main: "index",
            },
        },
    });
})(this);
