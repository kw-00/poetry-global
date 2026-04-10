import type { PersistedLanguage, PersistedPoemMetadata, PoemDTO } from "../api/client";
import { PoetryApiClient } from "../api/client";

export class InitializationError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "InitializationError";
  }
}

export type PoemsPageState = {
  items: PersistedPoemMetadata[];
  page: number;
  pageCount: number;
};

export class GlobalState {
  private languages: PersistedLanguage[] | null = null;
  private currentPoem: PoemDTO | null = null;
  private poemsPage: PoemsPageState | null = null;

  private api: PoetryApiClient
  constructor(api: PoetryApiClient) {
    this.api = api
  }

  // ---------------- LANGUAGES ----------------

  async getLanguages(): Promise<PersistedLanguage[]> {
    if (this.languages) return this.languages;

    try {
      const res = await this.api.getLanguages();
      this.languages = res.languages;
      return this.languages;
    } catch {
      throw new InitializationError("Failed to initialize languages");
    }
  }

  // ---------------- POEM ----------------

  async getPoemVersion(poemId: number, languageId: number): Promise<PoemDTO> {
    try {
      const res = await this.api.getPoem(poemId, languageId);
      this.currentPoem = res.poem;
      return this.currentPoem;
    } catch {
      throw new InitializationError("Failed to load poem");
    }
  }

  getCurrentPoem(): PoemDTO | null {
    return this.currentPoem;
  }

  // ---------------- POEMS PAGE ----------------

  async getPoems(author = "", title = "", page = 0): Promise<PersistedPoemMetadata[]> {
    try {
      const res = await this.api.searchPoems(title, author, page);

      this.poemsPage = {
        items: res.poemMetadata,
        page,
        pageCount: res.pageCount,
      };

      return this.poemsPage.items;
    } catch {
      throw new InitializationError("Failed to load poems");
    }
  }

  getCurrentPoemsPage(): PoemsPageState | null {
    return this.poemsPage;
  }

  // ---------------- OPTIONAL HELPERS ----------------

  clearPoem() {
    this.currentPoem = null;
  }

  clearCache() {
    this.languages = null;
    this.currentPoem = null;
    this.poemsPage = null;
  }
}

const globalState = new GlobalState(new PoetryApiClient(import.meta.env.VITE_API_URL));

export function useGlobalState() {
  return globalState
}
