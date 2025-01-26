// In development, always fetch from the network and do not enable offline support.
// This is because caching would make development more difficult (changes would not
// be reflected on the first load after each change).

// Remove the no-op fetch event handler
// self.addEventListener('fetch', () => { });

self.addEventListener('install', event => {
    console.log('Service worker installing...');
    // Perform install steps
});

self.addEventListener('activate', event => {
    console.log('Service worker activating...');
    // Perform activate steps
});
