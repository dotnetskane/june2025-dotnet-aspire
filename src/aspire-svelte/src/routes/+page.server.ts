import type { PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ fetch }) => {

    const baseUrl = process.env.services__apiservice__http__0 || process.env.services__apiservice__https__0 || 'http://localhost:5325';
    if (!baseUrl) {
        throw new Error('API service base URL is not defined in environment variables.');
    }

    const response = await fetch(`${baseUrl}/todos`);
    if (!response.ok) {
        throw new Error('Failed to fetch todos from API service.');
    }

    const todos = await response.json();

    return {
        serverMessage: 'This is server-side data fetched from +page.server.ts',
        todos
    };
};