import React, { useEffect, useState } from "react";
import { useGlobalState } from '@/global-state/global-state';
import type { PersistedPoemMetadata } from "../api/client";

export function SearchPanel({
  onSelectPoem,
}: {
  onSelectPoem: (poem: PersistedPoemMetadata) => void;
}) {
  const state = useGlobalState();

  const [title, setTitle] = useState("");
  const [author, setAuthor] = useState("");

  const [page, setPage] = useState(0);
  const [pageCount, setPageCount] = useState(0);

  const [results, setResults] = useState<PersistedPoemMetadata[]>([]);

  async function search(p = page) {
    try {
      if (title.trim() === "") {
        alert("Title is required");
      }
      const data = await state.getPoems(author, title, p);
      setResults(data);

      const meta = state.getCurrentPoemsPage();
      setPageCount(meta?.pageCount ?? 0);

      if (data.length === 0) {
        alert("No poems found");
      }
    } catch {
      alert("Failed to fetch poems");
    }
  }

  function changePage(newPage: number) {
    if (newPage < 0 || newPage >= pageCount) {
      alert("Page out of range");
      return;
    }

    setPage(newPage);
    search(newPage);
  }


  return (
    <div className="panel">
      <h2>Search Poems</h2>

      <div className="row">
        <input
          className="input"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <input
          className="input"
          placeholder="Author"
          value={author}
          onChange={(e) => setAuthor(e.target.value)}
        />
        <button
          className="button"
          onClick={() => {
            setPage(0);
            search(0);
          }}
        >
          Search
        </button>
      </div>

      <div className="results">
        {results.map((p) => (
          <div
            key={p.id}
            className="resultItem"
            onClick={() => onSelectPoem(p)}
          >
            <strong>{p.title}</strong> — {p.author}
          </div>
        ))}
      </div>

      <div className="row pageSwitch">
        <button className="button" onClick={() => changePage(page - 1)}>
          Prev
        </button>

        <span className="grow">
          {pageCount > 0 ? `Page ${page + 1} / ${pageCount}` : "--------"}
        </span>

        <button className="button" onClick={() => changePage(page + 1)}>
          Next
        </button>
      </div>
    </div>
  );
}