// Export utility functions for Export.razor page

// Download file from stream
async function downloadFileFromStream(fileName, streamReference) {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName ?? 'export.pdf';
    document.body.appendChild(link);
    link.click();
    
    URL.revokeObjectURL(url);
    link.remove();
}

// Initialize export page
function initializeExportPage() {
    console.log('Export page initialized');
    
    // Add event listeners for buttons if needed
    const buttons = document.querySelectorAll('.btn, .preset-btn, .btn-download');
    buttons.forEach(button => {
        button.addEventListener('click', function() {
            console.log('Button clicked:', this.textContent);
        });
    });
}

// Call initialize when document is ready
document.addEventListener('DOMContentLoaded', initializeExportPage);
