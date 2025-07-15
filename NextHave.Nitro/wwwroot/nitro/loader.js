window.loadNitro = () => {
    if (window.nitroLoaded)
        return;

    const config = document.createElement("script");
    config.src = "/nitro/nitro-config.js";

    const mainModule = document.createElement("script");
    mainModule.src = "/nitro/index-Dse5yibP.js";
    mainModule.type = "module";

    document.head.appendChild(config);
    document.head.appendChild(mainModule);

    window.nitroLoaded = true;
};