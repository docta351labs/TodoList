import { render, screen } from '@testing-library/react';
import App from '../App';

describe('App', () => {
  it('renders Get started header', () => {
    render(<App />);
    expect(screen.getByText(/Get started/i)).toBeInTheDocument();
  });
});
