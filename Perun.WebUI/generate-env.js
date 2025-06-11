const fs = require('fs');
const path = require('path');

// Setting up environment file

if (process.env.API_URL == null || process.env.API_URL == '') {
    console.error('API_URL environment variable is not set. Ignoring environment file generation.');
    process.exit(0);
}

const templatePath = path.resolve(__dirname, 'src/environments/environment.ts');
let templateContent = fs.readFileSync(templatePath, 'utf-8');

templateContent = templateContent.replace('${API_URL}', process.env.API_URL);

const outputPath = path.resolve(__dirname, 'src/environments/environment.ts');
fs.writeFileSync(outputPath, templateContent, 'utf-8');

console.log('Environment file generated successfully.');

// Setting up background image

if (process.env.BACKGROUND == 'onboarding') {
    fs.unlink("./public/bg.png", ()=>console.info("Background file removed"))
    fs.rename("./public/bg-onboarding.png", "./public/bg.png", ()=>console.info("Background file replaced with onboarding background"))
 } else {
    console.info('BACKGROUND environment variable is not set or not equal to onboarding. Ignoring backround file replacement.');
 }
