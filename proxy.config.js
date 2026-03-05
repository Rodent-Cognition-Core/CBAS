const fs = require('fs');
const path = require('path');

// Vanilla .env parser - avoids adding dependencies like dotenv
const envPath = path.resolve(__dirname, '.env');
if (fs.existsSync(envPath)) {
    const envContent = fs.readFileSync(envPath, 'utf8');
    envContent.split(/\r?\n/).forEach(line => {
        // Ignore comments and empty lines
        if (line && !line.startsWith('#')) {
            const [key, ...valueParts] = line.split('=');
            if (key && valueParts.length > 0) {
                const value = valueParts.join('=').trim().replace(/^["']|["']$/g, '');
                process.env[key.trim()] = value;
            }
        }
    });
}

const target = process.env.PROXY_TARGET;

if (!target) {
    console.error("[Proxy] ERROR: PROXY_TARGET environment variable is not set!");
    console.error("[Proxy] Please set PROXY_TARGET in your .env file or environment variables.");
} else {
    console.log(`[Proxy] Routing requests to: ${target}`);
}

module.exports = {
    "/api": {
        target: target || "",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    },
    "/connect": {
        target: target || "",
        secure: false,
        changeOrigin: true,
        logLevel: "debug"
    }
};
