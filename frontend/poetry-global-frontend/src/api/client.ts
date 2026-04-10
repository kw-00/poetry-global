

export class PoetryApiClient {
  // GET api/Poem/search?title=&author=&page=
  searchPoems(title = "", author = "", page = 0) {
    const params = new URLSearchParams({
      title,
      author,
      page: String(page),
    });

    return this.get<SearchResponse>(`/api/poem/search?${params}`);
  }

  // GET api/Poem/{poemId}/{languageId}
  getPoem(poemId: number, languageId: number) {
    return this.get<GetPoemResponse>(
      `/api/poem/${poemId}/${languageId}`
    );
  }

  // GET api/Language
  getLanguages() {
    return this.get<GetAllLanguagesResponse>(`/api/language`);
  }

  private baseUrl: string
  constructor(baseUrl: string) {
    this.baseUrl = baseUrl
  }

  private async get<T>(url: string): Promise<T> {
    const res = await fetch(`${this.baseUrl}${url}`);
    if (!res.ok) throw new Error(await res.text());
    return res.json();
  }
}

export interface PersistedPoemMetadata {
  // adjust fields to your real model
  id: number;
  title: string;
  author: string;
}

export interface PoemDTO {
  id: number;
  title: string;
  versionText: string;
  languageId: number;
}

export interface PersistedLanguage {
  id: number;
  code: string;
}

// responses
type SearchResponse = {
  poemMetadata: PersistedPoemMetadata[];
  pageCount: number;
};

type GetPoemResponse = {
  poem: PoemDTO;
};

type GetAllLanguagesResponse = {
  languages: PersistedLanguage[];
};