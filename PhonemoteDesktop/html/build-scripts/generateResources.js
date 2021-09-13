// This script puts the .html files in ../dist/ into the project resource file
// so that all pages are included within the exe/application

// Load in config file
const config = require("./config").config;

// Load in requires
const fs = require("fs");
const path = require("path");
const xml2js = require("xml2js");

// Get input dir
const inputDir = config.htmlOutputFolder;

// Get resx file
const resxFile = config.resxFile;

// Addresses to files are different in .resx files
const inResxAddress = config.projectToOutputAddress.replace(/\//g, '\\');

console.log("adding files to " +  resxFile);

// Create XML praser
const parser = new xml2js.Parser({explicitArray: false});

// Read .resx file
fs.readFile(resxFile, (err, xmlData) => {
    if (err) {
        throw err;
    }
    console.log("reading .resx file");
    
    // Parse .resx file
    parser.parseStringPromise(xmlData).then((parsedXML) => {

        console.log("parsed xml");

        // This array contains a list of pages
        // It then gets stored in the .resx file
        // This lets the program switch between pages
        // more easily
        let pageList = [];

        // array of data to add to the .resx
        let dataPageAddress = [];

        // Read inlined html dir
        fs.readdirSync(inputDir, { withFileTypes: true })
            .filter(dirent => dirent.isFile())
            .map(dirent => dirent.name)
            .filter(file => path.extname(file) === ".html")
            .forEach(file => {

            const fileName = path.parse(file).name;
            
            // Create new xml entry
            let newEntry = {
                '$': {
                    'name': fileName,
                    'type': 'System.Resources.ResXFileRef, System.Windows.Forms'
                },
                "value": `${inResxAddress}${file};System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=${config.resxPublicKeyToken};utf-8`
            };

            // Add page to page list
            pageList.push(fileName);
                
            // Add resource to array of resource imports
            dataPageAddress.push(newEntry);

        });

        let pageListEntry = {
            "$": {
                "name": "pageList",
                "xml:space": "preserve"
            },
            "value": pageList.join(",")
        };

        // If length is undefined, set it to an empty array
        if (parsedXML.root.data.length === undefined) {
            parsedXML.root.data = [];
        }

        // Loop through data element in .resx, and remove existing inlined .html elements
        for (let i = 0; i < parsedXML.root.data.length; i++) {

            // If the data is linked to output html dir, remove it
            if (parsedXML.root.data[i].value.startsWith(inResxAddress)) {
                parsedXML.root.data.splice(i, 1);
                i--;
                continue;
            }

            // If the data has name pageList, remove it
            if (parsedXML.root.data[i]["$"].name === "pageList") {
                parsedXML.root.data.splice(i, 1);
                i--;
                continue;
            }
        } 

        // Re-add resources to .resx
        parsedXML.root.data.push(pageListEntry);
        dataPageAddress.forEach(element => {
            parsedXML.root.data.push(element);
        });

        const builder = new xml2js.Builder();
        const xmlBuilt = builder.buildObject(parsedXML);

        // Write new resx
        fs.writeFileSync(resxFile, xmlBuilt);

        console.log("regenerated .resx");

    }).catch((err) => {
        throw err;
    });
});