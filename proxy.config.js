module.exports = {
    "/api": {
        target: "https://staging.mousebytes.ca",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    },
    "/connect": {
        target: "https://staging.mousebytes.ca", 
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    }
};
