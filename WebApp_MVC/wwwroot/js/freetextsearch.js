//Whole function is littered with suggestions from both ChatGPT 4o and Claude AI, but the base is derived from teacher Hans Mattin-Lasseis tutorial on search for users, but this script is for a search where 
//projects, users AND clients can show up. 

function initSearch(config) {
    let activeIndex = -1;
    let debounceTimer;
    const searchContainer = document.getElementById(config.containerId);
    const input = document.getElementById(config.inputId);
    const results = document.getElementById(config.resultsId);

    if (!searchContainer || !input || !results ) {
        console.error("Search initialization failed: Missing elements", {
            searchContainer, input, results
        });
        return;
    }

    input.addEventListener('focus', () => {
        searchContainer.classList.add('focused');
        results.classList.add('focused');
        if (input.value.trim().length > 0) {
            performSearch(input.value.trim());
        }
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            searchContainer.classList.remove('focused');
            clearResults();
        }, 100);
    });


    input.addEventListener('input', () => {
        const query = input.value.trim();
        activeIndex = -1;

        clearTimeout(debounceTimer);

        if (query.length >= 3) {
            debounceTimer = setTimeout(() => {
                performSearch(query);
            }, 200);
        } else {
            clearResults();
        }
    });

    input.addEventListener('keydown', (e) => {
        const items = results.querySelectorAll('.search-item');
        if (items.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                activeIndex = (activeIndex + 1) % items.length;
                updateSelectedItem(items);
                break;

            case 'ArrowUp':
                e.preventDefault();
                activeIndex = (activeIndex - 1 + items.length) % items.length;
                updateSelectedItem(items);
                break;

            case 'Enter':
                e.preventDefault();
                if (activeIndex >= 0 && items[activeIndex]) {
                    items[activeIndex].click();
                }
                break;
        }
    });

    function updateSelectedItem(items) {
        items.forEach(item => item.classList.remove('active'));
        if (items[activeIndex]) {
            items[activeIndex].classList.add('active');
            items[activeIndex].scrollIntoView({ block: 'nearest' });
        }
    }

    function performSearch(query) {
        if (!query) return;

        const url = typeof config.searchUrl === 'function'
            ? config.searchUrl(query)
            : `${config.searchUrl}?query=${encodeURIComponent(query)}`;

        console.log("Attempting to fetch from URL:", url);  // Add this line


        fetch(url)
            .then(response => {
                console.log("Response status:", response.status);  // Add this line
                if (!response.ok) {
                    console.log("Response status:", response.status);  // Add this to debug
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                console.log("Response data:", data);  // Add this line
                generateSearchResults(data);
            })
            .catch(error => {
                console.error('Error fetching search results:', error);
                results.innerHTML = `<div class="search-error">Error loading results</div>`
                results.style.display = 'block';
            });
    }

    function generateSearchResults(response) {
        const itemsArray = response.data || [];
        const limitedArray = itemsArray.slice(0, 10);

        results.innerHTML = '';       

        if (itemsArray.length === 0) {
            const noResult = document.createElement('div');
            noResult.classList.add('search-item');
            noResult.textContent = config.emptyMessage || 'No results found';
            results.appendChild(noResult);

        } else {
            limitedArray.forEach(item => {
                const itemId = item.id;
                const entityType = item.entityType;
                const detailsUrl = item.detailsUrl;

                const resultItem = document.createElement('div');
                resultItem.classList.add('search-item');
                resultItem.dataset.id = item.id;
                resultItem.dataset.detailsUrl = detailsUrl; 

                const displayText = item[config.displayProperty] || item.DisplayText;

                resultItem.innerHTML = `<span>${displayText}</span>`;

                resultItem.addEventListener('click', (event) => {
                    event.stopPropagation();
                    console.log("Item clicked: id = " + itemId + "entitytype = " + entityType + "detailsUrl = " + detailsUrl + "full item: " + item);

                    openDetails(itemId, entityType, detailsUrl);
                });
                results.appendChild(resultItem);
            });
        }

        results.classList.add('visible');
        activeIndex = -1;
    }

    function clearResults() {
        results.classList.remove('visible');

        setTimeout(() => {
            results.innerHTML = '';
        }, 400); 
    }

    function openDetails(itemId, entityType, detailsUrl) {
        console.log("Opening details:", { itemId, entityType, detailsUrl });

        if (detailsUrl) {
            setTimeout(() => {
                window.location.href = detailsUrl;
            }, 50);
        } else {
            console.error("Details URL is undefined", { itemId, entityType });
        }
    }
}