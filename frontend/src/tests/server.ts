import { setupServer } from 'msw/node';
import { todoHandlers } from './handlers/todoHandlers';

export const server = setupServer(...todoHandlers);
