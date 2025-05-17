window.loadNitro = () => {
    if (window._nitroLoaded) return;

    const config = document.createElement("script");
    config.src = "/nitro/nitro-config.js";

    const mainModule = document.createElement("script");
    mainModule.src = "/nitro/index-C6nMtzsI.js";
    mainModule.type = "module";

    document.head.appendChild(config);
    document.head.appendChild(mainModule);

    window._nitroLoaded = true;
};

window.waitForNitro = () => {
    return new Promise((resolve) => {
        const check = () => {
            if (window.Nitro && window.Nitro.communication.isLoaded) resolve(true);
            else setTimeout(check, 100);
        };
        check();
    });
};