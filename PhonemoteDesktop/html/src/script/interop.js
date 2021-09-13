
const cs_initialize = function() {
    const title = document.getElementsByTagName("title")[0].text;

    cs_extern_initialize(title);
}

const cs_navigate = function(arg) {
    cs_extern_navigate(arg);
}

cs_initialize();