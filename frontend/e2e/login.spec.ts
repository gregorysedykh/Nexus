import { test, expect } from '@playwright/test';

test.describe('Login page', () => {
  test('shows the app title', async ({ page }) => {
    await page.goto('/');

    await expect(page.getByRole('heading', { name: 'Nexus' })).toBeVisible();
  });

  test('shows validation error when user id is empty', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: 'Login' }).click();

    await expect(page.getByText('User ID is required.')).toBeVisible();
  });

  test('shows validation error when user id is not a positive integer', async ({ page }) => {
    await page.goto('/');
    await page.getByLabel('User ID').fill('abc');
    await page.getByRole('button', { name: 'Login' }).click();

    await expect(page.getByText('User ID must be a positive number.')).toBeVisible();
  });
});
