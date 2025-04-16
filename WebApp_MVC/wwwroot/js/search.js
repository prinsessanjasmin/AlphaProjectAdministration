//This js is created from teacher Hans Mattin-Lasseis video instructions, then I got help from Claude AI to fix some issues
//because of how my poroject is built.

function initMemberSelector(config) {

    console.log("Initializing member selector with config:", config);
    console.log("Preselected members:", config.preselected);

    let activeMemberIndex = -1;
    let selectedIds = [];
    const selectedIdsInput = document.getElementById(config.selectedIds);

    function updateSelectedIdsInput() {
        if (selectedIdsInput) {
            console.log("Before update, value is:", selectedIdsInput.value);
            console.log("Updating with IDs:", selectedIds);
            selectedIdsInput.value = JSON.stringify(selectedIds);
            console.log("After update, value is:", selectedIdsInput.value);

            // Check if validateField exists before calling it
            if (typeof validateField === 'function') {
                validateField(selectedIdsInput);
            }
        } else {
            console.error("selectedIdsInput not found!");
        }
    }

    function addMemberElement(item) {

        console.log("Adding member element:", item);

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

        const displayContainerId = config.displayContainerId || 'edit-selected-member-display';
        console.log("Looking for display container with ID:", displayContainerId);

        const selectedMemberDisplay = document.getElementById(displayContainerId);
        console.log("Found display container:", selectedMemberDisplay);

        if (!selectedMemberDisplay) {
            console.error(`Display container '${displayContainerId}' not found!`);
            return;
        }
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
        console.log("Member added to display:", itemId);
    }


    function addMember(item) {
        addMemberElement(item);

        // Clear input and hide results
        memberInput.value = '';
        memberResults.style.display = 'none';

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
                memberResults.innerHTML = `<div class="search-error">Error loading results</div>`
                memberResults.style.display = 'block';
            });
    }

    function renderSearchResults(response) {
        memberResults.innerHTML = '';

        const itemsArray = response.data || [];

        if (itemsArray.length === 0) {
            const noResult = document.createElement('div');
            noResult.classList.add('no-results');
            noResult.textContent = config.emptyMessage || 'No results found';
            memberResults.appendChild(noResult);
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
                memberResults.appendChild(resultItem);

            });
        }
        memberResults.style.display = 'block';
        activeMemberIndex = -1;
    }

    function updateActiveItem(memberItems) {
        memberItems.forEach(item => item.classList.remove('active'));
        if (memberItems[activeMemberIndex]) {
            memberItems[activeMemberIndex].classList.add('active');
            memberItems[activeMemberIndex].scrollIntoView({ block: 'nearest' });
        }
    }

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
    const memberInput = document.getElementById(config.inputId);
    const memberResults = document.getElementById(config.resultsId);

    if (!memberContainer || !memberInput || !memberResults || !selectedIdsInput) {
        console.error("Member selector initialization failed: Missing elements", {
            memberContainer, memberInput, memberResults, selectedIds
        });
        return;
    }

    const existingMembers = memberContainer.querySelectorAll('.' + (config.memberClass || 'member'));
    existingMembers.forEach(el => el.remove());

    if (Array.isArray(config.preselected) && config.preselected.length > 0) {

        console.log("Found preselected members:", config.preselected.length);

        selectedIds = [];

        config.preselected.forEach(item => {

            console.log("Adding preselected member:", item);

            addMemberElement(item);
        });

        // Update the hidden input after adding all preselected members
        updateSelectedIdsInput();
    } else {

        console.log("No preselected members found");

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

    memberInput.addEventListener('focus', () => {
        memberContainer.classList.add('focused');
        memberResults.classList.add('focused');
        if (memberInput.value.trim().length > 0) {
            searchMember(memberInput.value.trim());
        }
    });

    memberInput.addEventListener('blur', () => {
        setTimeout(() => {
            memberContainer.classList.remove('focused');
            memberResults.classList.remove('focused');
        }, 100);
    });


    memberInput.addEventListener('input', () => {
        const memberQuery = memberInput.value.trim();
        activeMemberIndex = -1;

        if (memberQuery.length > 0) {
            searchMember(memberQuery);

        } else {
            memberResults.style.display = 'none';
            memberResults.innerHTML = '';
        }
    });

    memberInput.addEventListener('keydown', (e) => {
        const memberItems = memberResults.querySelectorAll('.search-item');
        if (memberItems.length === 0) return;

        switch (e.key) {
            case 'ArrowDown':
                e.preventDefault();
                activeMemberIndex = (activeMemberIndex + 1) % memberItems.length;
                updateActiveItem(memberItems);
                break;

            case 'ArrowUp':
                e.preventDefault();
                activeMemberIndex = (activeMemberIndex - 1) % memberItems.length;
                updateActiveItem(memberItems);
                break;

            case 'Enter':
                e.preventDefault();
                if (activeMemberIndex >= 0 && memberItems[activeMemberIndex]) {
                    memberItems[activeMemberIndex].click();
                }
                break;

            case 'Backspace':
                if (memberInput.value === '') {
                    removeLastMember();
                }
                break;
        }
    });


}



