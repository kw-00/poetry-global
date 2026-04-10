import React, { useState } from "react";
import { SearchPanel } from "./SearchPanel";
import { PoemPanel } from "./PoemPanel";
import type { PersistedPoemMetadata } from "../api/client";

export default function MainPage() {
  const [selectedPoem, setSelectedPoem] =
    useState<PersistedPoemMetadata | null>(null);

  return (
    <div className="layout">
      <SearchPanel onSelectPoem={setSelectedPoem} />
      <PoemPanel selectedPoem={selectedPoem} />
    </div>
  );
}