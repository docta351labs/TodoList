import { test, expect } from '@playwright/test';

test.describe('E2E Smoke Test Flow', () => {
  test('creates a list, manages item status transitions, and deletes list', async ({ page }) => {
    // Step 1: Navigate to homepage
    await page.goto('/');
    await expect(page).toHaveTitle(/Vite \+ React/i);

    // Step 2: Create a new list titled "Smoke Test List"
    await page.click('button:has-text("New List"), button:has-text("Create First List")');
    await page.fill('input#todo-list-title', 'Smoke Test List');
    await page.click('button[type="submit"]:has-text("Create List")');

    // Assert list card exists on dashboard
    const listCard = page.locator('div[role="button"]:has-text("Smoke Test List")');
    await expect(listCard).toBeVisible();

    // Step 3: Navigate into the list
    await listCard.click();
    await expect(page.locator('h1')).toHaveText('Smoke Test List');

    // Step 4: Add a new item titled "Test Item"
    await page.fill('input#item-title', 'Test Item');
    await page.fill('textarea#item-desc', 'Playwright automated test item');
    await page.selectOption('select#item-priority', 'High');
    await page.click('button[type="submit"]:has-text("Add Item")');

    // Assert item appears
    await expect(page.locator('h4:has-text("Test Item")')).toBeVisible();

    // Step 5: Click "Start" on item -> status becomes InProgress
    const startBtn = page.locator('button[aria-label="Start task"]');
    await expect(startBtn).toBeVisible();
    await startBtn.click();

    // Step 6: Click "Complete" on item -> status becomes Done
    const completeBtn = page.locator('button[aria-label="Complete task"]');
    await expect(completeBtn).toBeVisible();
    await completeBtn.click();

    // Assert completed badge appears
    await expect(page.locator('span:has-text("Completed")')).toBeVisible();

    // Step 7: Navigate back to Dashboard and delete the list
    await page.click('button:has-text("Back to Dashboard")');
    await expect(page.locator('h1')).toHaveText('My Task Lists');

    const deleteBtn = page.locator('button[aria-label="Delete Smoke Test List"]');
    await deleteBtn.click();

    // Click confirm delete
    await page.click('button:has-text("Confirm")');

    // Assert: list no longer appears on Dashboard
    await expect(page.locator('div[role="button"]:has-text("Smoke Test List")')).not.toBeVisible();
  });
});
