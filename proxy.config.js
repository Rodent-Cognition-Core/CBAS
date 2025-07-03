module.exports = {
    "/api": {
        target: "https://production.mousebytes.ca",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    },
    "/connect": {
        target: "https://production.mousebytes.ca", 
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    }
};
