import { expect, test } from '@playwright/test';

test.describe('Fullstack login flow', () => {
  test('logs in with a seeded backend user and opens home page', async ({ page }) => {
    await page.goto('/');

    await page.getByLabel('User ID').fill('1');
    await page.getByRole('button', { name: 'Login' }).click();

    await expect(page).toHaveURL(/\/home(\?.*)?$/);
    await expect(page.getByRole('heading', { name: 'Welcome to your Nexus home' })).toBeVisible();
    await expect(page.getByText('Signed in as user: 1')).toBeVisible();
  });
});
