import { Routes, Route } from 'react-router';
import { DashboardPage } from './pages/DashboardPage';
import { ListDetailPage } from './pages/ListDetailPage';
import { ErrorBoundary } from './components/ErrorBoundary/ErrorBoundary';

export function App() {
  return (
    <ErrorBoundary>
      <Routes>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/lists/:id" element={<ListDetailPage />} />
      </Routes>
    </ErrorBoundary>
  );
}

export default App;
