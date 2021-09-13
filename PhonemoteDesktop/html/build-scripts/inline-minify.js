// This script consumes the .html files from ../src/ inlines and minifies them
// then places them in ../dist/

// Load in config file
const config = require("./config").config;

// Load in requires
const fs = require("fs");
const path = require("path");
const inline = require("web-resource-inliner");
const minify = require("html-minifier").minify;

const inputDir = config.htmlInputFolder;
const outputDir = config.htmlOutputFolder;

const htmlInlineOptions = {
    maxImageInlineSizeKB: 8
}

// Clear resources content so that old files do not get included in Resources.resx
fs.readdirSync(outputDir).forEach(file => {
    fs.unlinkSync(outputDir + file);
});

fs.readdir(inputDir, { withFileTypes: true }, (err, result) => {
    if (err) {
        throw err;
    }
    
    const htmlFiles = result
                    // Filter out folders
                    .filter(dirent => dirent.isFile())
                    // Replace dirent objects with file name
                    .map(dirent => dirent.name)
                    // Get only html files
                    .filter(file => path.extname(file) === ".html");

    // For each .html file, inline and minify
    htmlFiles.forEach(file => {
        
        console.log("inlining => " + file);

        fs.readFile(inputDir + file, "utf-8", (err, content) => {
            if (err) {
                throw err;
            }

            // Create inline html string with custom options
            inline.html({
                fileContent: content,
                images: htmlInlineOptions.maxImageInlineSizeKB,
                svgs: htmlInlineOptions.maxImageInlineSizeKB,
                relativeTo: inputDir
            }, function (err, inlinedContent) {
                if (err) {
                    throw err;
                }

                // The inlined content is stored now in inlinedContent
                console.log("inlined => " + file);
                console.log("minifying => " + file);

                const minifiedContent = minify(inlinedContent, {
                    caseSensitive: false,
                    collapseBooleanAttributes: true,
                    collapseWhitespace: true,
                    decodeEntities: true,
                    html5: true,
                    minifyCSS: true,
                    minifyJS: true,
                    removeComments: true,
                    removeEmptyAttributes: true,
                    removeScriptTypeAttributes: true,
                    removeStyleLinkTypeAttributes: true,
                    trimCustomFragments: true,
                    useShortDoctype: true
                });

                fs.writeFile(outputDir + file, minifiedContent, (err) => {
                    if (err) {
                        throw err;
                    }
                    console.log("saving file to => " + outputDir + file);
                });
            });
        });
    });
});