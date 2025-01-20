function completeQuest() {
    fetch('/Home/CompleteQuest', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update streak and XP display
                document.getElementById('streak-count').textContent = data.streak;
                document.getElementById('xp-reward').textContent = data.xpGained;

                // Update header XP display
                document.getElementById('current-xp').textContent = data.newXP;
                document.getElementById('xp-to-next').textContent = data.xpToNext;
                document.getElementById('level-display').textContent = `Level: ${data.currentLevel}`;

                // Update progress bar
                const progressPercentage = (data.newXP * 100) / data.xpToNext;
                document.getElementById('xp-progress-bar').style.width = `${progressPercentage}%`;

                // Disable the complete button and update text
                const button = document.getElementById('complete-quest');
                button.disabled = true;
                button.classList.remove('btn-primary');
                button.classList.add('btn-success');
                button.textContent = 'Completed Today!';
            }
        })
        .catch(error => console.error('Error:', error));
}
