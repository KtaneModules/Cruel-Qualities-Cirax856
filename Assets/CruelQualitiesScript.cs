using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KModkit;

public class CruelQualitiesScript : MonoBehaviour
{
    public KMBombInfo bombInfo;
    public KMBombModule module;
#pragma warning disable 0108
    public KMAudio audio;
#pragma warning restore 0108
    public KMColorblindMode colorblindMode;

    public KMSelectable chordButton;
    public KMSelectable selectButton;
    public KMSelectable submitButton;
    public KMSelectable octaveButton;
    public GameObject octaveArrow;

    public TextMesh[] colorblindTexts;

    public TextMesh binaryNumber;
    private int chosenBinaryNumber;

    public GameObject[] lights;
    public GameObject[] blinkerLights;
    public SpriteRenderer[] lightLights;
    public TextMesh[] arrows;

    private bool isInNotePlaybackAnimation = false;
    private bool isSolved = false;

    public Material greenLightMaterial;
    public Material yellowLightMaterial;
    public Material redLightMaterial;
    public Color greenLight = new Color(0, 1f, 0, 1f);
    public Color yellowLight = new Color(1f, 1f, 0, 1f);
    public Color redLight = new Color(1f, 0, 0, 1f);
    private bool isOctaveUp = false;

    private string[] notes = new string[] {"A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"};

    private int randomColorIndex;

    private int chosenChord;
    private int chosenRoot;
    private List<string> chosenNotesList = new List<string>();
    private List<string> solutionNotesList = new List<string>();
    private string[] chosenNotes;
    private string[] solutionNotes;
    private string[] chordsColors;
    private int solutionChord;
    private int solutionRoot;
    private string[] allChords;
    private string chord;
    private int[] chordsDistances;
    private int[] solutionChordDistances;
    private int noteIndex;
    bool isCorrect = true;

    private string[] chordsNormal = new string[]
    {
        "6/9",
        "Δ13",
        "-11",
        "Δsus2(add6)",
        "ob9",
        "+Δ#9",
        "-13#11",
        "+7sus4#9",
        "ø11b9",
        "ø#13",
        "Δ9sus4",
        "-Δ9#11"
    };
    private string[] chordsSpecial = new string[]
    {
        "Q",
        "NΔ",
        "E",
        "LY",
        "LO",
        "M",
        "P",
        "A",
        "F",
        "T",
        "S",
        "AS"
    };
    private int[][] chordsNormalDistances = new int[][]
    {
        new int[] {2, 2, 3, 2, 3}, // 6/9
        new int[] {2, 2, 1, 2, 2, 2, 1}, // Δ13
        new int[] {2, 1, 2, 2, 3, 2}, // -11
        new int[] {2, 5, 2, 2, 1}, // Δsus2(add6)
        new int[] {1, 2, 3, 4, 2}, // ob9
        new int[] {3, 1, 4, 3, 1}, // +Δ#9
        new int[] {2, 1, 3, 1, 1, 2, 2}, // -13#11
        new int[] {3, 2, 3, 2, 2}, // +7sus4#9
        new int[] {1, 2, 2, 1, 4, 2}, // ø11b9
        new int[] {2, 1, 2, 1, 3, 1, 2}, // ø#13
        new int[] {2, 3, 2, 4, 1}, // Δ9sus4
        new int[] {2, 1, 3, 1, 4, 1} // -Δ9#11
    };
    private int[][] chordsSpecialDistances = new int[][]
    {
        new int[] {3, 2, 3, 2, 2}, // Q
        new int[] {3, 5, 3, 1}, // NΔ
        new int[] {1, 3, 3, 2, 3}, // E
        new int[] {2, 2, 2, 1, 2, 2, 1}, // LY
        new int[] {1, 2, 2, 1, 2, 2, 2}, // LO
        new int[] {2, 2, 2, 3, 1, 2}, // M
        new int[] {2, 1, 3, 2, 1, 3}, // P
        new int[] {2, 1, 2, 1, 2, 1, 2, 1}, // A
        new int[] {4, 4, 1, 2, 1}, // F
        new int[] {3, 3, 4, 2}, // T
        new int[] {3, 1, 2, 1, 3, 1, 1}, // S
        new int[] {2, 2, 2, 2, 2, 2} // AS
    };
    private string[] chordsNormalColors = new string[]
    {
        "ygyyy", // 6/9
        "ygygygy", // Δ13
        "ygygyy", // -11
        "yyyyy", // Δsus2(add6)
        "ygyyy", // ob9
        "ygyyy", // +Δ#9
        "ygygygy", // -13#11
        "ygyyy", // +7sus4#9
        "ygygyy", // ø11b9
        "ygygyyg", // ø#13
        "ygyyy", // Δ9sus4
        "ygygyy" // -Δ9#11
    };
    private string[] chordsSpecialColors = new string[]
    {
        "ygygy", // Q
        "yyyy", // NΔ
        "yggyy", // E
        "ygygygy", // LY
        "ygygygy", // LO
        "yygygy", // M
        "ygyyyy", // P
        "ygygygyg", // A
        "ygygy", // F
        "ygyy", // T
        "ygygygy", // 
        "yyyygy" // 
    };

    private bool isPurpleSpawned = false;
    private bool isPurpleRoot = false;

    private int chosenNote1;
    private int chosenNote2;
    private int chosenNote3;
    private int chosenNote4;
    private int chosenNote5;

    private int currentLight = 0;

    static int moduleIdCount = 1;
#pragma warning disable 0414
    int moduleId;
#pragma warning restore 0414

#pragma warning disable 0414
    private Coroutine rotateCoroutine;
    private Coroutine rotateOctaveCoroutine;
    private Coroutine notePlaybackCoroutine;
    private Coroutine strikeAnimationCoroutine;
#pragma warning restore 0414

    void Start()
    {
        moduleId = moduleIdCount++;

        chordButton.OnInteract += delegate () { chordButtonPress(); return false; };
        selectButton.OnInteract += delegate () { selectButtonPress(); return false; };
        submitButton.OnInteract += delegate () { submitButtonPress(); return false; };
        octaveButton.OnInteract += delegate () { octaveButtonPress(); return false; };

        for(int i = 0; i < 12; i++)
        {
            arrows[i].text = "";
            lights[i].SetActive(false);

            colorblindTexts[i].GetComponent<Renderer>().enabled = false;
        }

        chosenBinaryNumber = Random.Range(0, 5) % 2;
        binaryNumber.text = chosenBinaryNumber.ToString();

        chosenChord = Random.Range(0, 11);
        chosenRoot = Random.Range(0, 11);

        noteIndex = chosenRoot;

        if (chosenBinaryNumber == 0)
        {
            allChords = chordsNormal;

            chordsColors = chordsNormalColors;
            chordsDistances = chordsNormalDistances[chosenChord];
            chord = chordsNormal[chosenChord];
        }
        else
        {
            allChords = chordsSpecial;

            chordsColors = chordsSpecialColors;
            chordsDistances = chordsSpecialDistances[chosenChord];
            chord = chordsSpecial[chosenChord];
        }

        for (int i = 0; i < chordsDistances.Length; i++)
        {
            chosenNotesList.Add(notes[noteIndex]);
            noteIndex += chordsDistances[i];
            if (noteIndex > 11) noteIndex -= 12;
        }

        chosenNotesList.Add(notes[noteIndex]);

        Debug.LogFormat("[Cruel Qualities #{0}] Displayed chord is {1} with the root note {2}.", moduleId, chord, notes[chosenRoot]);

        chosenNotes = chosenNotesList.ToArray();

        // arrows

        // spawning normal
        for(int i = 0; i < chosenNotes.Length - 1; i++)
        {
            arrows[System.Array.IndexOf(notes, chosenNotes[i])].text = "▲";

            if (chordsColors[chosenChord][i] == 'y') arrows[System.Array.IndexOf(notes, chosenNotes[i])].color = new Color(1f, 1f, 0, 1f);
            else arrows[System.Array.IndexOf(notes, chosenNotes[i])].color = new Color(0, 1f, 0, 1f);
        }

        // spawning cyan
        for(int i = 0; i < chosenNotes.Length - 1; i++)
        {
            int shouldPlace = Random.Range(0, 3);
            if ((arrows[i].color == new Color(1f, 1f, 0, 1f)) && (arrows[nextArrow(i, 6)].color == new Color(1f, 1f, 1f, 1f)) && (shouldPlace == 0))
            {
                arrows[i].text = "";
                arrows[i].color = new Color(1f, 1f, 1f, 1f);

                arrows[nextArrow(i, 6)].text = "▲";
                arrows[nextArrow(i, 6)].color = new Color(0, 1f, 1f, 1f);

                break;
            }
        }

        // spawning purple
        for(int i = 0; i < chosenNotes.Length - 1; i++)
        {
            int shouldPlace = Random.Range(0, 3);
            if ((arrows[i].color == new Color(1f, 1f, 0, 1f)) && (shouldPlace == 0))
            {
                arrows[i].color = new Color(0.5f, 0, 0.5f, 1f);
                isPurpleSpawned = true;
                if (i == chosenRoot) isPurpleRoot = true;

                break;
            }
        }

        // spawning orange
        for(int i = 0; i < chosenNotes.Length - 1; i++)
        {
            int shouldPlace = Random.Range(0, 3);
            if ((arrows[i].color == new Color(1f, 1f, 0, 1f)) && (arrows[previousArrow(i, (bombInfo.GetBatteryCount() + bombInfo.GetPortCount()) % 12)].color == new Color(1f, 1f, 1f, 1f)) && (shouldPlace == 0))
            {
                arrows[i].text = "";
                arrows[i].color = new Color(1f, 1f, 1f, 1f);

                arrows[previousArrow(i, (bombInfo.GetBatteryCount() + bombInfo.GetPortCount()) % 12)].text = "▲";
                arrows[previousArrow(i, (bombInfo.GetBatteryCount() + bombInfo.GetPortCount()) % 12)].color = new Color(1f, (165f/255f), 0, 1f);

                break;
            }
        }

        // spawning gray
        for(int i = 0; i < chosenNotes.Length - 1; i++)
        {
            int shouldPlace = Random.Range(0, 3);
            if(shouldPlace == 0)
            {
                if((new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }.Contains(bombInfo.GetSerialNumberNumbers().Sum())) && (arrows[i].color == new Color(1f, 1f, 1f, 1f)))
                {
                    arrows[i].text = "▲";
                    arrows[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                }
                else
                {
                    if(((bombInfo.GetSerialNumberNumbers().Last() % 2) == 1) && (arrows[i].color == new Color(0, 1f, 0, 1f)))
                    {
                        arrows[i].text = "▲";
                        arrows[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                    else if(((bombInfo.GetSerialNumberNumbers().Last() % 2) == 0) && (arrows[i].color == new Color(1f, 1f, 0, 1f)))
                    {
                        arrows[i].text = "▲";
                        arrows[i].color = new Color(0.5f, 0.5f, 0.5f, 1f);
                    }
                }

                break;
            }
        }

        // spawning red
        for (int i = 0; i < chosenNotes.Length - 1; i++)
        {
            int shouldPlace = Random.Range(0, 2);
            if ((arrows[nextArrow(i)].color == new Color(1f, 1f, 0, 1f)) && (arrows[previousArrow(i)].color == new Color(1f, 1f, 0, 1f)) && (shouldPlace == 0))
            {
                arrows[nextArrow(i)].text = "";
                arrows[nextArrow(i)].color = new Color(1f, 1f, 1f, 1f);
                arrows[previousArrow(i)].text = "";
                arrows[previousArrow(i)].color = new Color(1f, 1f, 1f, 1f);

                arrows[i].text = "▲";
                arrows[i].color = new Color(1f, 0, 0, 1f);

                break;
            }
        }

        // spawning colorblind
        if(colorblindMode.ColorblindModeActive == true)
        {
            for(int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i].text == "▲")
                {
                    colorblindTexts[i].GetComponent<Renderer>().enabled = true;

                    // colors:
                    Color yellow = new Color(1f, 1f, 0, 1f);
                    Color green = new Color(0, 1f, 0, 1f);
                    Color cyan = new Color(0, 1f, 1f, 1f);
                    Color red = new Color(1f, 0, 0, 1f);
                    Color purple = new Color(0.502f, 0, 0.502f, 1f);
                    Color orange = new Color(1f, (165f/255f), 0, 1f);
                    Color gray = new Color(0.502f, 0.502f, 0.502f, 1f);

                    if (arrows[i].color == yellow)
                    {
                        colorblindTexts[i].text = "Y";
                    }
                    else if (arrows[i].color == green)
                    {
                        colorblindTexts[i].text = "G";
                    }
                    else if (arrows[i].color == cyan)
                    {
                        colorblindTexts[i].text = "C";
                    }
                    else if (arrows[i].color == red)
                    {
                        colorblindTexts[i].text = "R";
                    }
                    else if ((Mathf.Abs(arrows[i].color.r - purple.r) <= 0.1f) && (Mathf.Abs(arrows[i].color.g - purple.g) <= 0.1f) && (Mathf.Abs(arrows[i].color.b - purple.b) <= 0.1f))
                    {
                        colorblindTexts[i].text = "P";
                    }
                    else if (arrows[i].color == orange)
                    {
                        colorblindTexts[i].text = "O";
                    }
                    else if ((Mathf.Abs(arrows[i].color.r - gray.r) <= 0.1f) && (Mathf.Abs(arrows[i].color.g - gray.g) <= 0.1f) && (Mathf.Abs(arrows[i].color.b - gray.b) <= 0.1f))
                    {
                        colorblindTexts[i].text = "A";
                    } else { Debug.Log($"Arrows is {arrows[i].color} and color gray is {gray}"); }
                }
            }
        }

        // quality root conversion

        // root to quality
        solutionChord = chosenRoot;

        // quality to root
        solutionRoot = chosenChord;

        // creating answer array
        noteIndex = solutionRoot;

        if (isPurpleSpawned && isPurpleRoot)
        {
            noteIndex = nextArrow(noteIndex);
        }
        else if (isPurpleSpawned)
        {
            noteIndex = previousArrow(noteIndex);
        }

        if(chosenBinaryNumber == 0)
        {
            solutionChordDistances = chordsSpecialDistances[solutionChord];
            allChords = chordsSpecial;
            chordsColors = chordsSpecialColors;
        }
        else
        {
            solutionChordDistances = chordsNormalDistances[solutionChord];
            allChords = chordsNormal;
            chordsColors = chordsNormalColors;
        }

        for (int i = 0; i < solutionChordDistances.Length; i++)
        {
            solutionNotesList.Add(notes[noteIndex]);
            noteIndex += solutionChordDistances[i];
            if (noteIndex > 11) noteIndex -= 12;
        }

        solutionNotes = solutionNotesList.ToArray();

        // sorting arrays
        System.Array.Sort(solutionNotes, (a, b) =>
        {
            int indexA = System.Array.IndexOf(notes, a);
            int indexB = System.Array.IndexOf(notes, b);
            return indexA.CompareTo(indexB);
        });

        // logging answer
        if (isPurpleSpawned && isPurpleRoot)
        {
            Debug.LogFormat("[Cruel Qualities #{0}] Purple arrow is present and is the root of the chord, shift the answer up by a semitone.", moduleId);
        }
        else if (isPurpleSpawned)
        {
            Debug.LogFormat("[Cruel Qualities #{0}] Purple arrow is present, but is not the root of the chord, shift the answer down by a semitone.", moduleId);
        }

        Debug.LogFormat("[Cruel Qualities #{0}] Solution chord is {1} with the root note {2}.", moduleId, allChords[solutionChord], notes[solutionRoot]);
    }

    private void chordButtonPress()
    {
        if(!isInNotePlaybackAnimation)
        {
            rotateCoroutine = StartCoroutine(rotateWheel());
            currentLight++;
            if (currentLight == 12)
            {
                currentLight = 0;
            }
        }
    }

    private IEnumerator rotateWheel()
    {
        audio.PlaySoundAtTransform("Wheel Turn", transform);

        for (int i = 0; i < 30; i++)
        {
            chordButton.GetComponent<Transform>().Rotate(0, -1, 0);
            yield return new WaitForSeconds(0.007f);
        }
    }

    private void selectButtonPress()
    {
        if(!isInNotePlaybackAnimation)
        {
            if (isOctaveUp)
            {
                audio.PlaySoundAtTransform(notes[currentLight] + "up", transform);
                blinkerLights[currentLight].GetComponent<Renderer>().material = greenLightMaterial;
                lightLights[currentLight].color = greenLight;
                lights[currentLight].SetActive(!lights[currentLight].activeSelf);
            }
            else
            {
                audio.PlaySoundAtTransform(notes[currentLight], transform);
                blinkerLights[currentLight].GetComponent<Renderer>().material = yellowLightMaterial;
                lightLights[currentLight].color = yellowLight;
                lights[currentLight].SetActive(!lights[currentLight].activeSelf);
            }
        }
    }

    private IEnumerator NotePlaybackAnimation(string[] inputtedNotes, string inputtedColors)
    {
        isInNotePlaybackAnimation = true;

        for(int i = 0; i < blinkerLights.Length; i++)
        {
            audio.PlaySoundAtTransform(notes[i], transform);
            blinkerLights[i].GetComponent<Renderer>().material = redLightMaterial;
            lightLights[i].color = redLight;
            lights[i].SetActive(true);

            yield return new WaitForSeconds(0.05f);
        }

        for(int i = 0; i < blinkerLights.Length; i++)
        {
            audio.PlaySoundAtTransform(notes[i] + "up", transform);
            blinkerLights[i].GetComponent<Renderer>().material = yellowLightMaterial;
            lightLights[i].color = yellowLight;
            lights[i].SetActive(false);

            yield return new WaitForSeconds(0.05f);
        }

        isInNotePlaybackAnimation = false;

        if (isCorrect)
        {
            for (int i = 0; i < blinkerLights.Length; i++)
            {
                audio.PlaySoundAtTransform(notes[i] + "up", transform);
                blinkerLights[i].GetComponent<Renderer>().material = greenLightMaterial;
                lightLights[i].color = greenLight;
                lights[i].SetActive(true);
            }

            isSolved = true;
            audio.PlaySoundAtTransform("Solve", transform);
            Debug.LogFormat("[Cruel Qualities #{0}] Correct! Successfully submitted the correct chord!", moduleId);
            module.HandlePass();
        }
        else
        {
            strikeAnimationCoroutine = StartCoroutine(strikeAnimation(inputtedNotes, inputtedColors));
        }
    }

    private IEnumerator strikeAnimation(string[] inputtedNotes, string inputtedColors)
    {
        isInNotePlaybackAnimation = true;

        for (int i = 0; i < blinkerLights.Length; i++)
        {
            blinkerLights[i].GetComponent<Renderer>().material = redLightMaterial;
            lightLights[i].color = redLight;
            lights[i].SetActive(true);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < blinkerLights.Length; i++)
        {
            blinkerLights[i].GetComponent<Renderer>().material = redLightMaterial;
            lightLights[i].color = redLight;
            lights[i].SetActive(false);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < blinkerLights.Length; i++)
        {
            blinkerLights[i].GetComponent<Renderer>().material = redLightMaterial;
            lightLights[i].color = redLight;
            lights[i].SetActive(true);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < blinkerLights.Length; i++)
        {
            blinkerLights[i].GetComponent<Renderer>().material = redLightMaterial;
            lightLights[i].color = redLight;
            lights[i].SetActive(false);
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < blinkerLights.Length; i++)
        {
            blinkerLights[i].GetComponent<Renderer>().material = yellowLightMaterial;
            lightLights[i].color = yellowLight;
        }

        isInNotePlaybackAnimation = false;

        audio.PlaySoundAtTransform("Strike", transform);
        Debug.LogFormat("[Cruel Qualities #{0}] Incorrect! Submitted notes {1}, expected {2} (order may be different). Submitted colors are {3}, expected {4} (encoded, while y is yellow and g is green).", moduleId, string.Join(", ", inputtedNotes), string.Join(", ", solutionNotes), inputtedColors, chordsColors[solutionChord]);
        module.HandleStrike();

        isCorrect = true;
    }

    private void submitButtonPress()
    {
        if(!isInNotePlaybackAnimation && !isSolved)
        {
            List<string> inputtedNotesList = new List<string>();
            string[] inputtedNotes;
            string[] unsortedInputtedNotes;

            for (int i = 0; i < lights.Length; i++)
            {
                if (lights[i].activeSelf)
                {
                    inputtedNotesList.Add(notes[i]);
                }
            }

            inputtedNotes = inputtedNotesList.ToArray();
            unsortedInputtedNotes = inputtedNotes;

            System.Array.Sort(inputtedNotes, (a, b) =>
            {
                int indexA = System.Array.IndexOf(notes, a);
                int indexB = System.Array.IndexOf(notes, b);
                return indexA.CompareTo(indexB);
            });

            string inputtedColors = "";

            for (int i = 0; i < unsortedInputtedNotes.Length; i++)
            {
                if (blinkerLights[System.Array.IndexOf(notes, unsortedInputtedNotes[i])].GetComponent<Renderer>().sharedMaterial == yellowLightMaterial)
                {
                    inputtedColors += "y";
                }
                else if (blinkerLights[System.Array.IndexOf(notes, unsortedInputtedNotes[i])].GetComponent<Renderer>().sharedMaterial == greenLightMaterial)
                {
                    inputtedColors += "g";
                }
                else
                {
                    inputtedColors += "?";
                }
            }

            inputtedNotes = inputtedNotesList.ToArray();

            for (int i = 0; i < arrows.Length; i++)
            {
                if (((lights[i].activeSelf) && !(solutionNotes.Contains(notes[i]))) || (!(lights[i].activeSelf) && (solutionNotes.Contains(notes[i]))))
                {
                    isCorrect = false;
                }
            }

            if (isCorrect != false) isCorrect = checkColorEqual(inputtedColors);

            notePlaybackCoroutine = StartCoroutine(NotePlaybackAnimation(inputtedNotes, inputtedColors));
        }
    }

    private bool checkColorEqual(string inputtedColors)
    {
        for (int j = 0; j < chordsColors[solutionChord].Length; j++)
        {
            string permutation = "";

            for (int k = 0; k < chordsColors[solutionChord].Length; k++)
            {
                int index = (j + k) % chordsColors[solutionChord].Length;
                permutation += chordsColors[solutionChord][index];
            }

            if (permutation == inputtedColors)
            {
                return true;
            }
        }
        return false;
    }

    private void octaveButtonPress()
    {
        if(isOctaveUp)
        {
            audio.PlaySoundAtTransform("Octave Down", transform);
        }
        else
        {
            audio.PlaySoundAtTransform("Octave Up", transform);
        }

        rotateOctaveCoroutine = StartCoroutine(rotateOctaveArrow());
        isOctaveUp = !isOctaveUp;
    }

    private IEnumerator rotateOctaveArrow()
    {
        for (int i = 0; i < 30; i++)
        {
            octaveArrow.GetComponent<Transform>().Rotate(0, 0, 6);
            yield return new WaitForSeconds(0.002f);
        }
    }

    private int nextArrow(int current, int amount = 1)
    {
        int returnInt = current;
        for (int i = 0; i < amount; i++)
        {
            if (((returnInt + 1) < arrows.Length) && ((returnInt + 1) >= 0)) returnInt++;
            else returnInt = 0;
        }

        return returnInt;
    }

    private int previousArrow(int current, int amount = 1)
    {
        int returnInt = current;
        for(int i = 0; i < amount; i++)
        {
            if (((returnInt - 1) < arrows.Length) && ((returnInt - 1) >= 0)) returnInt--;
            else returnInt = arrows.Length - 1;
        }

        return returnInt;
    }

    // twitch plays

    public bool TPColorblind = false;
#pragma warning disable 0414
    readonly private string TwitchHelpMessage = "Use \"!{0} rotate [amount]\" to rotate the wheel clockwise some amount of times (the default is 1). Use \"!{0} select\" to press the select button. Use \"!{0} octave\" to press the octave button. Use \"!{0} submit\" to submit your solution. Use \"!{0} sequence [sequence]\", to submit an entire sequence. Ex. \"!{0} sequence octave select rotate rotate rotate select submit\". Toggle colorblind mode using \"!{0} colorblind\".";
#pragma warning restore 0414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();

        if (command.StartsWith("rotate"))
        {
            string editedRotate = command.Remove(0, 6);
            bool invalid = false;
            int amount = 1;

            if (editedRotate.Length > 1)
            {
                editedRotate = editedRotate.Remove(0, 1);

                for (int i = 0; i < editedRotate.Length; i++)
                {
                    if (!(char.IsDigit(editedRotate[i])))
                    {
                        yield return "sendtochaterror!h Please input a valid amount of times you wish to rotate the wheel.";
                        invalid = true;
                    }
                }

                if (invalid == false)
                {
                    amount = System.Int32.Parse(editedRotate);
                }
            }

            yield return null;
            for (int i = 0; i < amount; i++)
            {
                yield return null;
                chordButton.OnInteract();
            }
        }
        else if (command.StartsWith("select"))
        {
            yield return null;
            selectButton.OnInteract();
        }
        else if (command.StartsWith("submit"))
        {
            yield return null;
            submitButton.OnInteract();
        }
        else if (command.StartsWith("octave"))
        {
            yield return null;
            octaveButton.OnInteract();
        }
        else if (command.StartsWith("sequence"))
        {
            if (command.Remove(0, 8).Length == 0)
            {
                yield return "sendtochaterror!h Please input a sequence.";
            }
            else
            {
                yield return null;
                string editedSequence = command.Remove(0, 9);
                while (editedSequence.Length != 0)
                {
                    if (editedSequence.StartsWith("rotate"))
                    {
                        yield return null;
                        chordButton.OnInteract();
                        if (editedSequence.Length > 6) editedSequence = editedSequence.Remove(0, 7);
                        else editedSequence = editedSequence.Remove(0, 6);
                        yield return new WaitForSeconds(0.2f);
                    }
                    else if (editedSequence.StartsWith("select"))
                    {
                        yield return null;
                        selectButton.OnInteract();
                        if (editedSequence.Length > 6) editedSequence = editedSequence.Remove(0, 7);
                        else editedSequence = editedSequence.Remove(0, 6);
                        yield return new WaitForSeconds(0.2f);
                    }
                    else if (editedSequence.StartsWith("submit"))
                    {
                        yield return null;
                        submitButton.OnInteract();
                        if (editedSequence.Length > 6) editedSequence = editedSequence.Remove(0, 7);
                        else editedSequence = editedSequence.Remove(0, 6);
                        yield return new WaitForSeconds(0.2f);
                    }
                    else if (editedSequence.StartsWith("octave"))
                    {
                        yield return null;
                        octaveButton.OnInteract();
                        if (editedSequence.Length > 6) editedSequence = editedSequence.Remove(0, 7);
                        else editedSequence = editedSequence.Remove(0, 6);
                        yield return new WaitForSeconds(0.2f);
                    }
                    else
                    {
                        editedSequence = "";
                        yield return "sendtochaterror!h Invalid command at some point of the message.";
                    }
                    Debug.Log(editedSequence);
                }
            }
        }
        else if (command.StartsWith("colorblind"))
        {
            yield return null;
            ToggleColorblind();
        }
    }

    private void ToggleColorblind()
    {
        if(TPColorblind == false)
        {
            for (int i = 0; i < arrows.Length; i++)
            {
                if (arrows[i].text == "▲")
                {
                    colorblindTexts[i].GetComponent<Renderer>().enabled = true;

                    // colors:
                    Color yellow = new Color(1f, 1f, 0, 1f);
                    Color green = new Color(0, 1f, 0, 1f);
                    Color cyan = new Color(0, 1f, 1f, 1f);
                    Color red = new Color(1f, 0, 0, 1f);
                    Color purple = new Color(0.502f, 0, 0.502f, 1f);
                    Color orange = new Color(1f, (165f / 255f), 0, 1f);
                    Color gray = new Color(0.502f, 0.502f, 0.502f, 1f);

                    if (arrows[i].color == yellow)
                    {
                        colorblindTexts[i].text = "Y";
                    }
                    else if (arrows[i].color == green)
                    {
                        colorblindTexts[i].text = "G";
                    }
                    else if (arrows[i].color == cyan)
                    {
                        colorblindTexts[i].text = "C";
                    }
                    else if (arrows[i].color == red)
                    {
                        colorblindTexts[i].text = "R";
                    }
                    else if ((Mathf.Abs(arrows[i].color.r - purple.r) <= 0.1f) && (Mathf.Abs(arrows[i].color.g - purple.g) <= 0.1f) && (Mathf.Abs(arrows[i].color.b - purple.b) <= 0.1f))
                    {
                        colorblindTexts[i].text = "P";
                    }
                    else if (arrows[i].color == orange)
                    {
                        colorblindTexts[i].text = "O";
                    }
                    else if ((Mathf.Abs(arrows[i].color.r - gray.r) <= 0.1f) && (Mathf.Abs(arrows[i].color.g - gray.g) <= 0.1f) && (Mathf.Abs(arrows[i].color.b - gray.b) <= 0.1f))
                    {
                        colorblindTexts[i].text = "A";
                    }
                    else { Debug.Log($"Arrows is {arrows[i].color} and color gray is {gray}"); }
                }
            }

            TPColorblind = true;
        } else
        {
            for (int i = 0; i < 12; i++)
            {
                colorblindTexts[i].GetComponent<Renderer>().enabled = false;
            }

            TPColorblind = false;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }

        if (isOctaveUp) octaveButton.OnInteract();

        for(int i = 0; i < currentLight; i++)
        {
            chordButton.OnInteract();
            yield return new WaitForSeconds(0.05f);
        }

        if(isPurpleSpawned && isPurpleRoot)
        {
            for (int i = 0; i < nextArrow(solutionRoot); i++)
            {
                chordButton.OnInteract();
                yield return new WaitForSeconds(0.05f);
            }
        }
        else if(isPurpleSpawned)
        {
            for (int i = 0; i < previousArrow(solutionRoot); i++)
            {
                chordButton.OnInteract();
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (int i = 0; i < solutionRoot; i++)
            {
                chordButton.OnInteract();
                yield return new WaitForSeconds(0.05f);
            }
        }

        for(int j = 0; j < solutionNotes.Length; j++)
        {
            for (int i = 0; i < solutionChordDistances[j]; i++)
            {
                chordButton.OnInteract();
                yield return new WaitForSeconds(0.05f);
            }

            if ((chordsColors[solutionChord][j] == 'g' && !isOctaveUp) || (chordsColors[solutionChord][j] == 'y' && isOctaveUp))
            {
                octaveButton.OnInteract();
                yield return new WaitForSeconds(0.1f);
            }

            selectButton.OnInteract();

            yield return new WaitForSeconds(0.1f);
        }

        submitButton.OnInteract();
    }
}
