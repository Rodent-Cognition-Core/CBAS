module.exports = {
    "/api": {
        target: "http://localhost:5000",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    },
    "/connect": {
        target: "http://localhost:5000", 
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    }
};
