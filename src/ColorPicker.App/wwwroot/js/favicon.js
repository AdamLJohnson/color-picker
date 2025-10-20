/**
 * Favicon utility functions for dynamic favicon updates
 */

export function updateFavicon(hexColor) {
    // Create SVG favicon as a colored square
    const svg = `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">
            <rect width="100" height="100" fill="${hexColor}" rx="10"/>
        </svg>
    `;

    // Convert SVG to data URI
    const dataUri = 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(svg);

    // Find or create favicon link element
    let faviconLink = document.querySelector('link[rel="icon"]');
    
    if (!faviconLink) {
        // Create new favicon link if it doesn't exist
        faviconLink = document.createElement('link');
        faviconLink.rel = 'icon';
        document.head.appendChild(faviconLink);
    }

    // Update the favicon href
    faviconLink.href = dataUri;
}

export function updateFaviconWithCircle(hexColor) {
    // Create SVG favicon as a colored circle
    const svg = `
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">
            <circle cx="50" cy="50" r="50" fill="${hexColor}"/>
        </svg>
    `;

    // Convert SVG to data URI
    const dataUri = 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(svg);

    // Find or create favicon link element
    let faviconLink = document.querySelector('link[rel="icon"]');
    
    if (!faviconLink) {
        // Create new favicon link if it doesn't exist
        faviconLink = document.createElement('link');
        faviconLink.rel = 'icon';
        document.head.appendChild(faviconLink);
    }

    // Update the favicon href
    faviconLink.href = dataUri;
}

