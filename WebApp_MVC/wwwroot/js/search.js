//This js is created from teacher Hans Mattin-Lasseis video instructions, then I got help from Claude AI to fix some issues
//because of how my poroject is built.



function initMemberSelector(config) {
    let activeIndex = -1; 
    let selectedIds = [];
    const selectedIdsInput = document.getElementById(config.selectedIds);

    if (selectedIdsInput && selectedIdsInput.value) {
        try {
            const existingIds = JSON.parse(selectedIdsInput.value);
            if (Array.isArray(existingIds)) {
                selectedIds = existingIds;
            }
        } catch (e) {
            console.error("Error parsing selected IDs", e);
        }
    }

    const memberContainer = document.getElementById(config.containerId);
    const input = document.getElementById(config.inputId);
    const results = document.getElementById(config.resultsId);

    if (!memberContainer || !input || !results || !selectedIdsInput) {
        console.error("Member selector initialization failed: Missing elements", {
            memberContainer, input, results, selectedIds
        });
        return;
    }

    const existingMembers = memberContainer.querySelectorAll('.' + (config.memberClass || 'member'));
    existingMembers.forEach(el => el.remove());

    if (Array.isArray(config.preselected) && config.preselected.length > 0) {
        // We're starting fresh - clear selectedIds array
        selectedIds = [];

        config.preselected.forEach(item => {
            // Create visual element AND update selectedIds array
            addMemberElement(item);
        });

        // Update the hidden input after adding all preselected members
        updateSelectedIdsInput();
    } else {
        // Try to load IDs from hidden input
        try {
            if (selectedIdsInput.value) {
                selectedIds = JSON.parse(selectedIdsInput.value);
            }
        } catch (e) {
            console.error("Error parsing selected IDs", e);
            selectedIds = [];
        }
    }

    if (selectedIds.length === 0 && selectedIdsInput && selectedIdsInput.value) {
        try {
            const existingIds = JSON.parse(selectedIdsInput.value);
            if (Array.isArray(existingIds)) {
                selectedIds = existingIds;
            }
        } catch (e) {
            console.error("Error parsing selected IDs", e);
        }
    }

    input.addEventListener('focus', () => {
        memberContainer.classList.add('focused');
        results.classList.add('focused');
        if (input.value.trim().length > 0) {
            searchMember(input.value.trim());
        }
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            memberContainer.classList.remove('focused');
            results.classList.remove('focused');
        }, 100);
    });


    input.addEventListener('input', () => {
        const query = input.value.trim();
        activeIndex = -1;

        if (query.length > 0) {
            searchMember(query);

        } else {
            results.style.display = 'none';
            results.innerHTML = '';
        }
    });

    input.addEventListener('keydown', (e) => {
        const items = results.querySelectorAll('.search-item');
        if (items.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                activeIndex = (activeIndex + 1) % items.length;
                updateActiveItem(items);
                break;

            case 'ArrowUp':
                e.preventDefault();
                activeIndex = (activeIndex - 1) % items.length;
                updateActiveItem(items);
                break;

            case 'Enter':
                e.preventDefault();
                if (activeIndex >= 0 && items[activeIndex]) {
                    items[activeIndex].click();
                }
                break;

            case 'Backspace':
                if (input.value === '') {
                    removeLastMember();
                }
                break;
        }
    });

    function updateActiveItem(items) {
        items.forEach(item => item.classList.remove('active'));
        if (items[activeIndex]) {
            items[activeIndex].classList.add('active');
            items[activeIndex].scrollIntoView({ block: 'nearest' });
        }
    }

    function searchMember(query) {
        if (!query) return;

        const url = typeof config.searchUrl === 'function'
            ? config.searchUrl(query)
            : `${config.searchUrl}?term=${encodeURIComponent(query)}`;

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => renderSearchResults(data))
            .catch(error => {
                console.error('Error fetching search results:', error);
                results.innerHTML = `<div class="search-error">Error loading results</div>`
                results.style.display = 'block';
            });
    }

    function renderSearchResults(response) {
        results.innerHTML = ''; 

        const itemsArray = response.data || [];

        if (itemsArray.length === 0) {
            const noResult = document.createElement('div');
            noResult.classList.add('no-results');
            noResult.textContent = config.emptyMessage || 'No results found';
            results.appendChild(noResult);
        } else {

            itemsArray.forEach(item => {
                const itemId = item.Id || item.id || item.ID;

                if (selectedIds.includes(parseInt(item.id)) ||
                    selectedIds.includes(item.id)) {
                    return;
                }

                const resultItem = document.createElement('div');
                resultItem.classList.add('search-item');
                resultItem.dataset.id = item.Id;

                const displayText = item[config.displayProperty] || item.name || 'Unknown';

                if (config.memberClass === 'member' && item[config.imageProperty]) {

                    const imagePath = item[config.imageProperty].startsWith('http')
                        ? item[config.imageProperty]
                        : (config.avatarFolder || '') + item[config.imageProperty];

                    resultItem.innerHTML =
                        `
                        <img class="user-avatar" src="${imagePath}" onerror="this.src='/ProjectImages/Icons/Avatar.svg'" />
                        <span>${displayText}</span>
                    `;
                } else {
                    resultItem.innerHTML = `<span>${displayText}</span>`;
                }
                   
                resultItem.addEventListener('click', () => addMember(item));
                results.appendChild(resultItem);

            });
        }
        results.style.display = 'block';
        activeIndex = -1;
    }

    function addMemberElement(item) {

        // Get the ID (handle different case variations)
        const itemId = item.Id || item.id;
        if (!itemId) {
            console.error("Cannot add member without ID:", item);
            return;
        }

        const numericId = parseInt(itemId);

        // For preselected items, we want to add even if they're in the array
        // But for user-selected items, we want to avoid duplicates
        if (!item._isPreselected &&
            (selectedIds.includes(numericId) || selectedIds.includes(itemId))) {
            return;
        }

        // Add to selectedIds array
        if (!isNaN(numericId)) {
            if (!selectedIds.includes(numericId)) {
                selectedIds.push(numericId);
            }
        } else {
            if (!selectedIds.includes(itemId)) {
                selectedIds.push(itemId);
            }
        }

        // Create the visual element
        const selectedMemberDisplay = document.getElementById('selected-member-display'); 

        const member = document.createElement('div');
        member.classList.add(config.memberClass || 'member');
        member.dataset.id = itemId;

        // Get display text and image path
        const displayProperty = config.displayProperty;
        const displayText = item[displayProperty] ||
            item[displayProperty.toLowerCase()] ||
            item[displayProperty.charAt(0).toUpperCase() + displayProperty.slice(1)] ||
            'Unknown';

        const imageProperty = config.imageProperty;
        const imagePath = item[imageProperty] ||
            item[imageProperty.toLowerCase()] ||
            item[imageProperty.charAt(0).toUpperCase() + imageProperty.slice(1)] ||
            '/ProjectImages/Icons/Avatar.svg';

        if (config.memberClass === 'member') {
            member.innerHTML = `
                <img class="user-avatar" src="${imagePath}" onerror="this.src='/ProjectImages/Icons/Avatar.svg'" />
                <span>${displayText}</span>
            `;
        } else {
            member.innerHTML = `<span>${displayText}</span>`;
        }

        // Add remove button
        const removeBtn = document.createElement('span');
        removeBtn.textContent = '×';
        removeBtn.classList.add('btn-remove');
        removeBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            selectedIds = selectedIds.filter(i => i !== numericId && i !== itemId);
            member.remove();
            updateSelectedIdsInput();
        });

        member.appendChild(removeBtn);
        selectedMemberDisplay.appendChild(member);
    }

    function addMember(item) {
        addMemberElement(item);

        // Clear input and hide results
        input.value = '';
        results.style.display = 'none';

        // Update hidden input
        updateSelectedIdsInput();
    }

    function removeLastMember() {
        const members = memberContainer.querySelectorAll(`.${config.memberClass || 'member'}`);
        if (members.length > 0) {
            const lastMember = members[members.length - 1];
            const id = lastMember.dataset.id;
            const numericId = parseInt(id);

            selectedIds = selectedIds.filter(i => i !== numericId && i !== id);
            lastMember.remove();
            updateSelectedIdsInput();
        }
    }

    function updateSelectedIdsInput() {

        if (selectedIdsInput) {
            selectedIdsInput.value = JSON.stringify(selectedIds);
            validateField(selectedIdsInput);
        }
    }
}