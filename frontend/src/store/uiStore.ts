import { create } from 'zustand';

interface UiState {
  isCreateListModalOpen: boolean;
  openCreateListModal: () => void;
  closeCreateListModal: () => void;
}

export const useUiStore = create<UiState>((set) => ({
  isCreateListModalOpen: false,
  openCreateListModal: () => set({ isCreateListModalOpen: true }),
  closeCreateListModal: () => set({ isCreateListModalOpen: false }),
}));
